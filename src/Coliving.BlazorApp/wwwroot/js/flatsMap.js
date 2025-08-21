window.FlatsMap = (function () {
  let map;
  let markers = [];
  const cacheKey = 'flats-geo-cache-v1';
  let geoCache = {}; // { addressKey: { lat, lon } }

  function loadCache() {
    try {
      geoCache = JSON.parse(localStorage.getItem(cacheKey)) || {};
    } catch {
      geoCache = {};
    }
  }

  function saveCache() {
    try {
      localStorage.setItem(cacheKey, JSON.stringify(geoCache));
    } catch {}
  }

  function addrKey(p) {
    return [p.address || '', p.city || '', p.country || ''].map(s => (s || '').trim()).filter(Boolean).join(', ');
  }

  async function geocode(address) {
    if (!address) return null;
    if (geoCache[address]) return geoCache[address];

    // Friendly throttle (Nominatim usage policy)
    await new Promise(r => setTimeout(r, 1100));

    const url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}&limit=1`;
    const res = await fetch(url, {
      headers: {
        'Accept': 'application/json',
        'User-Agent': 'VeganColiving/1.0 (contact@example.com)'
      }
    });
    if (!res.ok) return null;
    const data = await res.json();
    if (!Array.isArray(data) || data.length === 0) return null;

    const { lat, lon } = data[0];
    const point = { lat: parseFloat(lat), lon: parseFloat(lon) };
    geoCache[address] = point;
    saveCache();
    return point;
  }

  function popupHtml(p) {
    const img = p.imageUrl ? `<img src="${p.imageUrl}" style="width:100%;max-height:120px;object-fit:cover;border-radius:4px;margin-bottom:6px;"/>` : '';
    const price = isFinite(p.price) ? `<div><strong>Price:</strong> ${p.price.toFixed(0)}</div>` : '';
    const address = [p.address, p.city, p.country].filter(Boolean).join(', ');
    return `
      <div style="min-width:220px">
        ${img}
        <div style="font-weight:600;margin-bottom:4px">${p.title || 'Flat'}</div>
        <div style="color:#555">${address}</div>
        ${price}
      </div>
    `;
  }

  return {
    init: function (elementId) {
      loadCache();
      const el = document.getElementById(elementId);
      if (!el) {
        console.error('[FlatsMap] Container not found:', elementId);
        return false;
      }
      if (typeof L === 'undefined') {
        console.error('[FlatsMap] Leaflet (L) is not available.');
        el.innerHTML = '<div style="padding:1rem;color:#b00;background:#fee;border:1px solid #fbb">Map library failed to load.</div>';
        return false;
      }
      el.innerHTML = '';
      map = L.map(elementId, { zoomControl: true });
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
      }).addTo(map);
      map.setView([20, 0], 2); // world view by default
      return true;
    },

    clear: function () {
      markers.forEach(m => m.remove());
      markers = [];
    },

  plot: async function (points) {
      if (!map) {
        console.warn('[FlatsMap] plot called before init');
        return;
      }
      this.clear();

      const latlngs = [];

      for (const p of points || []) {
        const key = addrKey({ address: p.address, city: p.city, country: p.country });
        const geo = await geocode(key);
        if (!geo) continue;

        const marker = L.marker([geo.lat, geo.lon]).addTo(map);
        marker.bindPopup(popupHtml(p));
        markers.push(marker);
        latlngs.push([geo.lat, geo.lon]);
      }

      if (latlngs.length > 0) {
        const bounds = L.latLngBounds(latlngs);
        map.fitBounds(bounds.pad(0.2));
      }
    }
  }
})();
