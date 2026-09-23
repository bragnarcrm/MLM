// ScaleEngine Multi-Tenant White-Label SaaS Theming & Customization Engine
(() => {
  const DEFAULT_BRAND = {
    brandName: 'ScaleEngine',
    slogan: 'MLM PLATFORM',
    niche: 'wellness',
    primaryColor: '#0284c7',
    accentColor: '#38bdf8',
    currencySymbol: 'R',
    iconClass: 'bi-cpu-fill',
    heroTagline: 'High-Yield Partner Network & Automated Compensation Platform',
    products: [
      { name: 'Business Starter Kit', price: 499, desc: 'Welcome pack, catalog, promotional samples & backoffice license', isBusinessKit: true },
      { name: 'Herbal Tea Pack (30 Bags)', price: 199, desc: 'Daily natural botanical antioxidant and wellness blend', isBusinessKit: false },
      { name: 'Wellness & Detox Bundle', price: 799, desc: 'Complete 90-day supply and promotional materials', isBusinessKit: false }
    ]
  };

  const NICHE_PRESETS = {
    wellness: {
      brandName: 'Aura Vitality',
      slogan: 'NATURAL WELLNESS NETWORK',
      niche: 'wellness',
      primaryColor: '#10b981',
      accentColor: '#34d399',
      currencySymbol: 'R',
      iconClass: 'bi-heart-pulse-fill',
      heroTagline: 'Organic Herbal Infusions, Daily Vitality & High-Yield Partner Compensation',
      products: [
        { name: 'Aura Vitality Starter Pack', price: 499, desc: 'Official herbal onboarding pack, catalog & sample teas', isBusinessKit: true },
        { name: 'Herbal Detox Tea Pack (30 Bags)', price: 199, desc: 'Natural daily antioxidant and detox herbal blend', isBusinessKit: false },
        { name: '90-Day Full Wellness Bundle', price: 799, desc: 'Complete daytime energy and evening wellness tea supply', isBusinessKit: false }
      ]
    },
    beauty: {
      brandName: 'Luxe Glow Cosmetics',
      slogan: 'PREMIUM SKINCARE & BEAUTY',
      niche: 'beauty',
      primaryColor: '#e11d48',
      accentColor: '#fb7185',
      currencySymbol: 'R',
      iconClass: 'bi-gem',
      heroTagline: 'Luxury Botanical Skincare, Anti-Aging Serums & Affiliate Network',
      products: [
        { name: 'Luxe Glow Business Starter Kit', price: 599, desc: 'Cosmetics sample kit, tester palette, catalog & sales license', isBusinessKit: true },
        { name: 'Radiance Anti-Aging Serum (50ml)', price: 249, desc: 'Hyaluronic acid and vitamin C botanical glow formula', isBusinessKit: false },
        { name: 'Complete 5-Step Luxury Beauty Set', price: 899, desc: 'Cleanser, toner, serum, day cream & midnight repair balm', isBusinessKit: false }
      ]
    },
    crypto: {
      brandName: 'Apex FX & Crypto Academy',
      slogan: 'DIGITAL WEALTH & TRADING',
      niche: 'crypto',
      primaryColor: '#8b5cf6',
      accentColor: '#a78bfa',
      currencySymbol: '$',
      iconClass: 'bi-currency-bitcoin',
      heroTagline: 'Algorithmic Trading Signals, Financial Education & Tiered Network Rewards',
      products: [
        { name: 'Master Trader Academy License', price: 299, desc: 'Complete video curriculum, private Discord & trading software license', isBusinessKit: true },
        { name: 'VIP Trading Signal Bot Pack', price: 99, desc: 'Real-time AI forex and crypto breakout alerts', isBusinessKit: false },
        { name: 'Automated Trading Node License', price: 699, desc: 'Cloud bot automation software with lifetime priority support', isBusinessKit: false }
      ]
    },
    solar: {
      brandName: 'SolarPeak Clean Energy',
      slogan: 'RENEWABLE ENERGY NETWORK',
      niche: 'solar',
      primaryColor: '#f59e0b',
      accentColor: '#fbbf24',
      currencySymbol: 'R',
      iconClass: 'bi-sun-fill',
      heroTagline: 'Decentralized Solar Power Solutions, Inverters & Clean Energy Commissions',
      products: [
        { name: 'Solar Partner Starter Package', price: 1200, desc: 'Site assessment kit, marketing tools & certified partner license', isBusinessKit: true },
        { name: 'Eco Portable Backup Battery Kit', price: 450, desc: 'Emergency backup lighting and device charger module', isBusinessKit: false },
        { name: 'Smart Home Solar Station Bundle', price: 1800, desc: 'Complete 3kW inverter and battery storage distributor demo pack', isBusinessKit: false }
      ]
    },
    custom: {
      brandName: 'Custom Enterprise MLM',
      slogan: 'GLOBAL PARTNER NETWORK',
      niche: 'custom',
      primaryColor: '#0284c7',
      accentColor: '#38bdf8',
      currencySymbol: 'R',
      iconClass: 'bi-stars',
      heroTagline: 'Tailored Multi-Level Partner Marketing & Automated Commission Distribution',
      products: [
        { name: 'Enterprise Starter Package', price: 500, desc: 'Full business licensing, digital backoffice & product catalog', isBusinessKit: true },
        { name: 'Core Product Package', price: 200, desc: 'Standard distributor unit package', isBusinessKit: false },
        { name: 'Executive Master Bundle', price: 1000, desc: 'High-tier volume package with maximum commission leverage', isBusinessKit: false }
      ]
    }
  };

  // Get active brand configuration
  const getActiveBrand = () => {
    try {
      const stored = localStorage.getItem('scale_tenant_brand');
      if (stored) {
        return { ...DEFAULT_BRAND, ...JSON.parse(stored) };
      }
    } catch { }
    return DEFAULT_BRAND;
  };

  // Apply CSS custom variables and live DOM replacements
  const applyBrandToPage = (brand) => {
    if (!brand) brand = getActiveBrand();
    const root = document.documentElement;

    // 1. Dynamic CSS Variables
    root.style.setProperty('--brand-primary', brand.primaryColor);
    root.style.setProperty('--brand-accent', brand.accentColor);

    // 2. Injected Style Rule for buttons, badges, highlights
    let styleTag = document.getElementById('saas-dynamic-styles');
    if (!styleTag) {
      styleTag = document.createElement('style');
      styleTag.id = 'saas-dynamic-styles';
      document.head.appendChild(styleTag);
    }

    styleTag.textContent = `
      :root {
        --brand-primary: ${brand.primaryColor};
        --brand-accent: ${brand.accentColor};
      }
      .brand-badge, .panel-icon, .btn-primary-action, .btn-head-cart, .btn-checkout, .btn-save-bank, .btn-send-mail, .btn-generate-pin, .btn-checkout-direct, .btn-ai-send, button[type="submit"]:not(.btn-modal-danger) {
        background: ${brand.primaryColor} !important;
        border-color: ${brand.primaryColor} !important;
      }
      .brand-badge:hover, .btn-primary-action:hover, .btn-head-cart:hover, .btn-checkout:hover, .btn-save-bank:hover, .btn-send-mail:hover, .btn-generate-pin:hover, .btn-checkout-direct:hover, .btn-ai-send:hover {
        filter: brightness(0.88);
      }
      .badge-read-ticket, .badge-rank, .nav-ai-badge, .badge-model {
        background: ${brand.primaryColor} !important;
        color: #fff !important;
      }
      .portal-header-badge i, .brand-text strong, .brand-brand-title, .cart-toast a, .price-tag, .product-price {
        color: ${brand.primaryColor} !important;
      }
      .product-picker-card.selected, .product-card:has(input:checked) {
        border-color: ${brand.primaryColor} !important;
      }
      .saas-brand-name {
        color: inherit;
      }
    `;

    // 3. Replace Brand Text & Logos in Sidebar, Header, Title & Footer
    document.querySelectorAll('.brand-text strong, #brandNameDisplay, .landing-brand-name').forEach((el) => {
      el.textContent = brand.brandName;
    });

    document.querySelectorAll('.brand-text span, #brandSloganDisplay').forEach((el) => {
      el.textContent = brand.slogan;
    });

    document.querySelectorAll('.portal-header-badge, .header-platform-name').forEach((el) => {
      el.innerHTML = `<i class="${brand.iconClass}"></i> ${brand.brandName} Platform`;
    });

    document.querySelectorAll('.brand-badge i, .brand-mark-icon').forEach((icon) => {
      icon.className = `bi ${brand.iconClass}`;
    });

    // Update document title if it contains ScaleEngine
    if (document.title.includes('ScaleEngine')) {
      document.title = document.title.replace('ScaleEngine', brand.brandName);
    }

    // 4. Update Footer Copyright
    document.querySelectorAll('.portal-footer, footer p').forEach((footer) => {
      if (footer.textContent.includes('ScaleEngine') || footer.textContent.includes('All rights reserved')) {
        footer.textContent = `2026 © ${brand.brandName}. All rights reserved. Powered by ScaleEngine SaaS.`;
      }
    });

    // 5. Update Currency Symbols across Prices if set
    if (brand.currencySymbol && brand.currencySymbol !== 'R') {
      document.querySelectorAll('.currency-sym').forEach((el) => {
        el.textContent = brand.currencySymbol;
      });
    }
  };

  // Initialize SaaS Demo Modal HTML
  const injectSaaSDemoModal = () => {
    if (document.getElementById('saasDemoModal')) return;

    const modalHtml = `
      <div class="scale-modal-backdrop" id="saasDemoModal" aria-hidden="true" style="display:none; position:fixed; inset:0; z-index:999999; background:rgba(8,10,13,0.75); backdrop-filter:blur(4px); justify-content:center; align-items:center; padding:16px;">
        <div class="scale-modal-dialog" style="position:relative; width:100%; max-width:680px; max-height:92vh; overflow-y:auto; background:#fff; border-radius:14px; border:1px solid #cbd5e1; box-shadow:0 25px 60px -15px rgba(0,0,0,.4);">
          <div style="display:flex; justify-content:space-between; align-items:center; padding:18px 24px; border-bottom:1px solid #e2e8f0; background:#f8fafc;">
            <div style="display:flex; align-items:center; gap:10px;">
              <div style="width:36px; height:36px; border-radius:8px; background:#0f172a; color:#38bdf8; display:grid; place-items:center; font-size:18px;">
                <i class="bi bi-palette-fill"></i>
              </div>
              <div>
                <h2 style="margin:0; font-size:17px; font-weight:800; color:#0f172a;">SaaS Demo Rebrand &amp; White-Label Sandbox</h2>
                <small style="color:#64748b; font-size:12px;">Instantly customize company name, colors, industry niche &amp; products</small>
              </div>
            </div>
            <button type="button" id="closeSaaSModalBtn" style="border:0; background:transparent; font-size:24px; cursor:pointer; color:#64748b; line-height:1;">&times;</button>
          </div>

          <div style="padding:22px 24px;">
            <!-- Niche Presets Selector -->
            <label style="display:block; font-size:12.5px; font-weight:700; color:#334155; margin-bottom:8px; text-transform:uppercase; letter-spacing:.04em;">
              1. Choose MLM Niche / Industry Preset:
            </label>
            <div style="display:grid; grid-template-columns:repeat(auto-fit, minmax(130px, 1fr)); gap:8px; margin-bottom:18px;">
              <button type="button" class="saas-preset-btn" data-preset="wellness" style="padding:10px 8px; border:1px solid #cbd5e1; border-radius:8px; background:#f8fafc; cursor:pointer; text-align:center; font-size:12px; font-weight:700; color:#0f172a;">
                <i class="bi bi-heart-pulse-fill" style="color:#10b981; font-size:18px; display:block; margin-bottom:4px;"></i>
                Wellness / Tea
              </button>
              <button type="button" class="saas-preset-btn" data-preset="beauty" style="padding:10px 8px; border:1px solid #cbd5e1; border-radius:8px; background:#f8fafc; cursor:pointer; text-align:center; font-size:12px; font-weight:700; color:#0f172a;">
                <i class="bi bi-gem" style="color:#e11d48; font-size:18px; display:block; margin-bottom:4px;"></i>
                Beauty / Skincare
              </button>
              <button type="button" class="saas-preset-btn" data-preset="crypto" style="padding:10px 8px; border:1px solid #cbd5e1; border-radius:8px; background:#f8fafc; cursor:pointer; text-align:center; font-size:12px; font-weight:700; color:#0f172a;">
                <i class="bi bi-currency-bitcoin" style="color:#8b5cf6; font-size:18px; display:block; margin-bottom:4px;"></i>
                Crypto / FX
              </button>
              <button type="button" class="saas-preset-btn" data-preset="solar" style="padding:10px 8px; border:1px solid #cbd5e1; border-radius:8px; background:#f8fafc; cursor:pointer; text-align:center; font-size:12px; font-weight:700; color:#0f172a;">
                <i class="bi bi-sun-fill" style="color:#f59e0b; font-size:18px; display:block; margin-bottom:4px;"></i>
                Solar / Energy
              </button>
            </div>

            <!-- Custom Company Form -->
            <div style="display:grid; grid-template-columns:1fr 1fr; gap:14px; margin-bottom:16px;">
              <div>
                <label style="display:block; font-size:12.5px; font-weight:600; color:#334155; margin-bottom:5px;">Company / Brand Name *</label>
                <input id="saasInputName" style="width:100%; height:40px; padding:0 12px; border:1px solid #cbd5e1; border-radius:6px; font-size:13.5px;" value="ScaleEngine" required>
              </div>
              <div>
                <label style="display:block; font-size:12.5px; font-weight:600; color:#334155; margin-bottom:5px;">Slogan / Subtitle *</label>
                <input id="saasInputSlogan" style="width:100%; height:40px; padding:0 12px; border:1px solid #cbd5e1; border-radius:6px; font-size:13.5px;" value="MLM PLATFORM" required>
              </div>
            </div>

            <!-- Color Palette & Swatches -->
            <div style="margin-bottom:18px;">
              <label style="display:block; font-size:12.5px; font-weight:700; color:#334155; margin-bottom:8px; text-transform:uppercase; letter-spacing:.04em;">
                2. Theme Primary Color:
              </label>
              <div style="display:flex; align-items:center; gap:10px; flex-wrap:wrap;">
                <button type="button" class="saas-color-pill" data-color="#0284c7" data-accent="#38bdf8" style="width:34px; height:34px; border-radius:50%; background:#0284c7; border:2px solid #fff; box-shadow:0 0 0 1px #0284c7; cursor:pointer;" title="Royal Blue"></button>
                <button type="button" class="saas-color-pill" data-color="#10b981" data-accent="#34d399" style="width:34px; height:34px; border-radius:50%; background:#10b981; border:2px solid #fff; box-shadow:0 0 0 1px #10b981; cursor:pointer;" title="Emerald Green"></button>
                <button type="button" class="saas-color-pill" data-color="#8b5cf6" data-accent="#a78bfa" style="width:34px; height:34px; border-radius:50%; background:#8b5cf6; border:2px solid #fff; box-shadow:0 0 0 1px #8b5cf6; cursor:pointer;" title="Violet Purple"></button>
                <button type="button" class="saas-color-pill" data-color="#e11d48" data-accent="#fb7185" style="width:34px; height:34px; border-radius:50%; background:#e11d48; border:2px solid #fff; box-shadow:0 0 0 1px #e11d48; cursor:pointer;" title="Ruby Red"></button>
                <button type="button" class="saas-color-pill" data-color="#f59e0b" data-accent="#fbbf24" style="width:34px; height:34px; border-radius:50%; background:#f59e0b; border:2px solid #fff; box-shadow:0 0 0 1px #f59e0b; cursor:pointer;" title="Amber Gold"></button>
                <button type="button" class="saas-color-pill" data-color="#0f172a" data-accent="#64748b" style="width:34px; height:34px; border-radius:50%; background:#0f172a; border:2px solid #fff; box-shadow:0 0 0 1px #0f172a; cursor:pointer;" title="Obsidian Dark"></button>
                
                <label style="display:inline-flex; align-items:center; gap:6px; font-size:12px; color:#64748b; margin-left:8px; cursor:pointer;">
                  Custom: <input type="color" id="saasColorPicker" value="#0284c7" style="width:34px; height:34px; border:0; padding:0; border-radius:6px; cursor:pointer;">
                </label>
              </div>
            </div>

            <!-- Currency Symbol -->
            <div style="margin-bottom:18px;">
              <label style="display:block; font-size:12.5px; font-weight:600; color:#334155; margin-bottom:5px;">Platform Currency:</label>
              <select id="saasCurrencySelect" style="width:100%; height:40px; padding:0 12px; border:1px solid #cbd5e1; border-radius:6px; font-size:13.5px; background:#fff;">
                <option value="R">ZAR (R) &mdash; South African Rand</option>
                <option value="$">USD ($) &mdash; US Dollar</option>
                <option value="€">EUR (€) &mdash; Euro</option>
                <option value="£">GBP (£) &mdash; British Pound</option>
                <option value="₦">NGN (₦) &mdash; Nigerian Naira</option>
              </select>
            </div>

            <!-- Live Card Preview -->
            <div style="padding:16px 18px; border-radius:10px; background:#0f172a; color:#fff; display:flex; justify-content:space-between; align-items:center; margin-bottom:20px;">
              <div style="display:flex; align-items:center; gap:12px;">
                <div id="saasPreviewBadge" style="width:40px; height:40px; border-radius:8px; background:#0284c7; color:#fff; display:grid; place-items:center; font-size:20px;">
                  <i class="bi bi-cpu-fill" id="saasPreviewIcon"></i>
                </div>
                <div>
                  <strong id="saasPreviewName" style="display:block; font-size:16px; font-weight:800; color:#fff;">ScaleEngine</strong>
                  <small id="saasPreviewSlogan" style="color:#94a3b8; font-size:10px; letter-spacing:.1em; text-transform:uppercase;">MLM PLATFORM</small>
                </div>
              </div>
              <span id="saasPreviewPill" style="padding:4px 10px; border-radius:999px; background:#0284c7; color:#fff; font-size:11px; font-weight:700;">Live Brand Ready</span>
            </div>

            <!-- Actions -->
            <div style="display:flex; gap:10px;">
              <button type="button" id="saasApplyBtn" style="flex:2; height:46px; border:0; border-radius:6px; background:#0284c7; color:#fff; font-size:14px; font-weight:700; cursor:pointer; display:flex; align-items:center; justify-content:center; gap:8px;">
                <i class="bi bi-rocket-takeoff-fill"></i> Launch Custom Demo Now
              </button>
              <button type="button" id="saasResetBtn" style="flex:1; height:46px; border:1px solid #cbd5e1; border-radius:6px; background:#fff; color:#64748b; font-size:13px; font-weight:600; cursor:pointer;">
                Reset Default
              </button>
            </div>
          </div>
        </div>
      </div>
    `;

    document.body.insertAdjacentHTML('beforeend', modalHtml);

    // Setup modal listeners
    const modal = document.getElementById('saasDemoModal');
    const closeBtn = document.getElementById('closeSaaSModalBtn');
    const inputName = document.getElementById('saasInputName');
    const inputSlogan = document.getElementById('saasInputSlogan');
    const colorPicker = document.getElementById('saasColorPicker');
    const currencySelect = document.getElementById('saasCurrencySelect');
    const applyBtn = document.getElementById('saasApplyBtn');
    const resetBtn = document.getElementById('saasResetBtn');

    const previewName = document.getElementById('saasPreviewName');
    const previewSlogan = document.getElementById('saasPreviewSlogan');
    const previewBadge = document.getElementById('saasPreviewBadge');
    const previewPill = document.getElementById('saasPreviewPill');
    const previewIcon = document.getElementById('saasPreviewIcon');

    let currentSelectedColor = '#0284c7';
    let currentSelectedAccent = '#38bdf8';
    let currentIconClass = 'bi-cpu-fill';
    let currentNiche = 'wellness';
    let currentProducts = DEFAULT_BRAND.products;

    const updatePreview = () => {
      previewName.textContent = inputName.value || 'Custom MLM';
      previewSlogan.textContent = inputSlogan.value || 'NETWORK';
      previewBadge.style.background = currentSelectedColor;
      previewPill.style.background = currentSelectedColor;
      applyBtn.style.background = currentSelectedColor;
      previewIcon.className = `bi ${currentIconClass}`;
    };

    inputName.addEventListener('input', updatePreview);
    inputSlogan.addEventListener('input', updatePreview);

    // Preset Buttons
    document.querySelectorAll('.saas-preset-btn').forEach((btn) => {
      btn.addEventListener('click', () => {
        const presetKey = btn.dataset.preset;
        const preset = NICHE_PRESETS[presetKey];
        if (!preset) return;

        inputName.value = preset.brandName;
        inputSlogan.value = preset.slogan;
        currentSelectedColor = preset.primaryColor;
        currentSelectedAccent = preset.accentColor;
        currentIconClass = preset.iconClass;
        currentNiche = preset.niche;
        currentProducts = preset.products;
        currencySelect.value = preset.currencySymbol;
        colorPicker.value = preset.primaryColor;

        updatePreview();
      });
    });

    // Color Swatches
    document.querySelectorAll('.saas-color-pill').forEach((pill) => {
      pill.addEventListener('click', () => {
        currentSelectedColor = pill.dataset.color;
        currentSelectedAccent = pill.dataset.accent;
        colorPicker.value = currentSelectedColor;
        updatePreview();
      });
    });

    colorPicker.addEventListener('input', (e) => {
      currentSelectedColor = e.target.value;
      currentSelectedAccent = e.target.value;
      updatePreview();
    });

    closeBtn.addEventListener('click', () => {
      modal.style.display = 'none';
      modal.setAttribute('aria-hidden', 'true');
    });

    modal.addEventListener('click', (e) => {
      if (e.target === modal) {
        modal.style.display = 'none';
        modal.setAttribute('aria-hidden', 'true');
      }
    });

    // Apply button
    applyBtn.addEventListener('click', () => {
      const customBrand = {
        brandName: inputName.value.trim() || 'ScaleEngine',
        slogan: inputSlogan.value.trim() || 'MLM PLATFORM',
        niche: currentNiche,
        primaryColor: currentSelectedColor,
        accentColor: currentSelectedAccent,
        currencySymbol: currencySelect.value,
        iconClass: currentIconClass,
        products: currentProducts
      };

      localStorage.setItem('scale_tenant_brand', JSON.stringify(customBrand));
      applyBrandToPage(customBrand);

      // If store catalog exists on page, update store products in localStorage
      localStorage.setItem('scale_cart_preset', JSON.stringify(currentProducts));

      modal.style.display = 'none';
      modal.setAttribute('aria-hidden', 'true');

      // Refresh page if requested or notify
      alert(`🎉 Demo Rebranded to "${customBrand.brandName}"! Theme color and niche catalog have been applied.`);
      window.location.reload();
    });

    // Reset button
    resetBtn.addEventListener('click', () => {
      localStorage.removeItem('scale_tenant_brand');
      localStorage.removeItem('scale_cart_preset');
      applyBrandToPage(DEFAULT_BRAND);
      modal.style.display = 'none';
      window.location.reload();
    });
  };

  window.openSaaSDemoModal = () => {
    injectSaaSDemoModal();
    const modal = document.getElementById('saasDemoModal');
    if (modal) {
      modal.style.display = 'flex';
      modal.setAttribute('aria-hidden', 'false');
    }
  };

  // Auto-init on page load
  document.addEventListener('DOMContentLoaded', () => {
    injectSaaSDemoModal();
    applyBrandToPage();

    // Attach click handler to any button/link with class .btn-saas-demo or [data-action="open-saas-demo"]
    document.querySelectorAll('.btn-saas-demo, [data-action="open-saas-demo"]').forEach((el) => {
      el.addEventListener('click', (e) => {
        e.preventDefault();
        window.openSaaSDemoModal();
      });
    });
  });

  // Export to global window
  window.ScaleSaaS = {
    getActiveBrand,
    applyBrandToPage,
    openDemoModal: window.openSaaSDemoModal,
    presets: NICHE_PRESETS
  };
})();
