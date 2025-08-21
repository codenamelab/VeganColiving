(function(){
  // Wait for Leaflet and markercluster plugin
  function ready(){
    return typeof window.L !== 'undefined' && typeof window.L.markerClusterGroup === 'function';
  }
  function initMap(options){
    const el = document.getElementById(options.elementId || 'map');
    if(!el){ console.warn('Map element not found'); return; }

    const center = options.center || [59.9139, 10.7522];
    const zoom = options.zoom || 12;

    const map = L.map(el).setView(center, zoom);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    const listings = (options.listings||[]);
    const clusters = L.markerClusterGroup();
    listings.forEach(l=>{
      const m = L.marker([l.lat, l.lng]);
      const price = (l.price ?? 0).toLocaleString('no-NO');
      const rooms = l.rooms ?? 0;
      const title = l.title ?? '';
      m.bindPopup(`<strong>${title}</strong><br>${rooms} rom – ${price} kr/mnd`);
      clusters.addLayer(m);
    });
    map.addLayer(clusters);
  }

  window.HomesMap = {
    init: function(options){
      if(ready()) return initMap(options||{});
      let tries = 0;
      const iv = setInterval(()=>{
        tries++;
        if(ready()) { clearInterval(iv); initMap(options||{}); }
        else if(tries>50){ clearInterval(iv); console.error('Leaflet markercluster failed to load'); }
      }, 100);
    }
  };
})();
