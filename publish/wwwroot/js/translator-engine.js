// ScaleEngine Multilingual Real-Time Translation Engine
(() => {
  const DICTIONARY = {
    zu: { // isiZulu
      "Dashboard": "Ideshibhodi",
      "Register New Member": "Bhalisa Ilungu Elisha",
      "Team Structure": "Isakhiwo Seqembu",
      "Tree View": "Ukubuka Isihlahla",
      "Referral Tree": "Isihlahla Sokudlulisela",
      "My Referrals": "Abadluliselwe Bami",
      "Account": "I-akhawunti",
      "Profile": "Iphrofayili",
      "KYC Verification": "Ukuqinisekiswa Kwe-KYC",
      "Transaction Password": "Iphasiwedi Yokwenziwe",
      "Change Password": "Shintsha Iphasiwedi",
      "Activation": "Ukwenza Kusebenze",
      "My Income": "Imali Engenayo Yami",
      "All Commission": "Wonke Amakhomishini",
      "Direct Referral Bonus": "Ibhonasi Yokudlulisela Ngqo",
      "Level Bonus": "Ibhonasi Yezinga",
      "Monthly Salary": "Iholo Lenyanga",
      "Shopping": "Ukuthenga",
      "Business Kit": "Ikhithi Yebhizinisi",
      "Shopping Cart": "Inqola Yokuthenga",
      "Orders": "Ama-oda",
      "Ewallet": "I-Ewallet",
      "Summary": "Isifinyezo",
      "Withdraw Money": "Khipha Imali",
      "Add Bank Account": "Faka I-akhawunti Yasebhange",
      "Team Chat": "Ingxoxo Yeqembu",
      "Support Tickets": "Amathikithi Osekelo",
      "Messages": "Imilayezo",
      "AI Partner Assistant": "Umsizi We-AI Wezakwethu",
      "Logout": "Phuma",
      "Total Earned": "Ingqikithi Ezuziwe",
      "Total Withdrawal": "Ingqikithi Ekhishiwe",
      "Direct Referrals": "Abadluliselwe Ngqo",
      "Your Rank": "Izinga Lakho",
      "Invite": "Mema",
      "Notifications": "Izaziso",
      "Welcome back": "Siyakwamukela futhi",
      "Place Order": "Faka I-oda",
      "Proceed to Checkout": "Qhubekela Ekukhokheni",
      "Compliance Audit": "Ukuhlolwa Kokuhambisana",
      "Plan Builder": "Umakhi Wohlelo"
    },
    xh: { // isiXhosa
      "Dashboard": "Ideshibhodi",
      "Register New Member": "Bhalisa Ilungu Elitsha",
      "Team Structure": "Ulwakhiwo Lweqela",
      "Tree View": "Ukujonga Umthi",
      "My Referrals": "Abadluliselwe Bam",
      "Account": "Iakhawunti",
      "Profile": "Iprofayile",
      "Transaction Password": "Iphasiwedi Yentengiselwano",
      "My Income": "Ingeniso Yam",
      "All Commission": "Yonke Ikhomishini",
      "Monthly Salary": "Umvuzo Wenyanga",
      "Shopping": "Ukuthenga",
      "Shopping Cart": "Inqwelo Yokuthenga",
      "Orders": "Ii-odolo",
      "Ewallet": "I-Ewallet",
      "Withdraw Money": "Khupha Imali",
      "Add Bank Account": "Dibanisa Ibhanki",
      "Team Chat": "Incoko Yeqela",
      "Support Tickets": "Amatikiti Oncedo",
      "Messages": "Imiyalezo",
      "AI Partner Assistant": "Umncedisi We-AI",
      "Logout": "Phuma",
      "Total Earned": "Iyonke Eyenziweyo",
      "Total Withdrawal": "Iyonke Ekhutshiweyo",
      "Your Rank": "Inqanaba Lakho"
    },
    st: { // Sesotho
      "Dashboard": "Dashboto",
      "Register New Member": "Ngolisa Setho se Setjha",
      "Team Structure": "Sebopeho sa Sehlopha",
      "Tree View": "Pono ya Sefate",
      "My Referrals": "Ba fetiseditsweng ba Ka",
      "Account": "Akhaonto",
      "Profile": "Profaele",
      "My Income": "Meputso ya Ka",
      "Monthly Salary": "Moputso wa Kgwedi",
      "Shopping": "Ho Reka",
      "Shopping Cart": "Kariki ya ho Reka",
      "Orders": "Ditaelo",
      "Ewallet": "Ewallet",
      "Withdraw Money": "Hula Chelete",
      "Team Chat": "Puisano ya Sehlopha",
      "Support Tickets": "Ditekete tsa Tshehetso",
      "Messages": "Melaetsa",
      "AI Partner Assistant": "Mothusi wa AI",
      "Logout": "Tswa",
      "Total Earned": "Tsohle tse Fumanweng"
    },
    fr: { // Français
      "Dashboard": "Tableau de Bord",
      "Register New Member": "Inscrire un Membre",
      "Team Structure": "Structure de l'Équipe",
      "Tree View": "Vue Arborescente",
      "My Referrals": "Mes Filleuls",
      "Account": "Compte",
      "Profile": "Profil",
      "KYC Verification": "Vérification KYC",
      "Transaction Password": "Mot de Passe de Sécurité",
      "Change Password": "Changer Mot de Passe",
      "My Income": "Mes Revenus",
      "All Commission": "Toutes les Commissions",
      "Direct Referral Bonus": "Bonus de Parrainage Direct",
      "Level Bonus": "Bonus de Niveau",
      "Monthly Salary": "Salaire Mensuel",
      "Shopping": "Boutique",
      "Business Kit": "Kit de Démarrage",
      "Shopping Cart": "Panier d'Achat",
      "Orders": "Commandes",
      "Ewallet": "Portefeuille",
      "Summary": "Résumé",
      "Withdraw Money": "Retirer des Fonds",
      "Add Bank Account": "Ajouter Compte Bancaire",
      "Team Chat": "Chat d'Équipe",
      "Support Tickets": "Tickets d'Assistance",
      "Messages": "Boîte de Réception",
      "AI Partner Assistant": "Assistant IA Partenaire",
      "Logout": "Déconnexion",
      "Total Earned": "Total Gagné",
      "Total Withdrawal": "Total Retiré",
      "Direct Referrals": "Filleuls Directs",
      "Your Rank": "Votre Rang",
      "Invite": "Inviter",
      "Notifications": "Notifications",
      "Welcome back": "Bienvenue",
      "Place Order": "Passer Commande",
      "Proceed to Checkout": "Passer à la Caisse"
    },
    pt: { // Português
      "Dashboard": "Painel de Controle",
      "Register New Member": "Registar Novo Membro",
      "Team Structure": "Estrutura da Equipa",
      "Tree View": "Visualização em Árvore",
      "My Referrals": "Meus Indicados",
      "Account": "Conta",
      "Profile": "Perfil",
      "KYC Verification": "Verificação KYC",
      "Transaction Password": "Senha de Transação",
      "Change Password": "Alterar Senha",
      "My Income": "Meus Ganhos",
      "All Commission": "Todas as Comissões",
      "Monthly Salary": "Salário Mensal",
      "Shopping": "Loja",
      "Shopping Cart": "Carrinho de Compras",
      "Orders": "Pedidos",
      "Ewallet": "Carteira Digital",
      "Withdraw Money": "Levantar Dinheiro",
      "Add Bank Account": "Adicionar Conta Bancária",
      "Team Chat": "Chat de Equipa",
      "Support Tickets": "Tickets de Suporte",
      "Messages": "Mensagens",
      "AI Partner Assistant": "Assistente IA",
      "Logout": "Sair",
      "Total Earned": "Total Ganho",
      "Your Rank": "Seu Nível"
    }
  };

  const getSavedLang = () => localStorage.getItem('scale_lang') || 'en';

  const applyTranslation = (lang) => {
    if (!lang) lang = getSavedLang();
    localStorage.setItem('scale_lang', lang);

    if (lang === 'en') {
      document.querySelectorAll('[data-orig-text]').forEach((el) => {
        el.textContent = el.getAttribute('data-orig-text');
      });
      return;
    }

    const dict = DICTIONARY[lang];
    if (!dict) return;

    // Scan headings, links, buttons, spans, th, strong
    const targets = document.querySelectorAll('nav a span, nav a, .portal-header-action span, h1, h2, h3, h4, th, .metric span, .badge, .btn, .stat-pill span, strong, .nav-label, button');
    targets.forEach((el) => {
      if (el.children.length === 0) {
        const text = el.textContent.trim();
        if (dict[text]) {
          if (!el.hasAttribute('data-orig-text')) {
            el.setAttribute('data-orig-text', text);
          }
          el.textContent = dict[text];
        }
      }
    });
  };

  const injectLanguageSwitcher = () => {
    const headerRight = document.querySelector('.portal-header-right, .nav-actions');
    if (!headerRight || document.querySelector('#scaleLangSelector')) return;

    const current = getSavedLang();
    const selectHtml = `
      <div class="lang-selector-wrap" style="display:inline-flex; align-items:center; margin-right:4px;">
        <i class="bi bi-translate" style="color:#94a3b8; font-size:14px; margin-right:4px;"></i>
        <select id="scaleLangSelector" style="height:32px; padding:0 8px; border:1px solid rgba(255,255,255,0.18); border-radius:6px; background:#141820; color:#e2e8f0; font-size:11.5px; font-weight:700; outline:none; cursor:pointer;">
          <option value="en" ${current === 'en' ? 'selected' : ''}>🇿🇦 EN (English)</option>
          <option value="zu" ${current === 'zu' ? 'selected' : ''}>🇿🇦 ZU (isiZulu)</option>
          <option value="xh" ${current === 'xh' ? 'selected' : ''}>🇿🇦 XH (isiXhosa)</option>
          <option value="st" ${current === 'st' ? 'selected' : ''}>🇿🇦 ST (Sesotho)</option>
          <option value="fr" ${current === 'fr' ? 'selected' : ''}>🇫🇷 FR (Français)</option>
          <option value="pt" ${current === 'pt' ? 'selected' : ''}>🇵🇹 PT (Português)</option>
        </select>
      </div>
    `;

    headerRight.insertAdjacentHTML('afterbegin', selectHtml);

    document.querySelector('#scaleLangSelector')?.addEventListener('change', (e) => {
      applyTranslation(e.target.value);
    });
  };

  document.addEventListener('DOMContentLoaded', () => {
    injectLanguageSwitcher();
    if (getSavedLang() !== 'en') {
      setTimeout(() => applyTranslation(), 200);
    }
  });

  window.ScaleTranslator = {
    setLanguage: applyTranslation,
    getLanguage: getSavedLang,
    dictionary: DICTIONARY
  };
})();
