document.querySelectorAll('link[href*="portal-navigation.css?v=3"]').forEach((link) => link.remove());

document.querySelectorAll('a[href="login.html"]').forEach((link) => {
  if (link.textContent.includes('Logout')) {
    link.addEventListener('click', () => localStorage.removeItem('vitality.accessToken'));
  }
});

const headerAvatar = document.querySelector('#headerAvatar');
const headerName = document.querySelector('#headerName');
const headerRank = document.querySelector('#headerRank');
const headerNotifBadge = document.querySelector('#headerNotifBadge');

const accessToken = localStorage.getItem('vitality.accessToken');
if (accessToken) {
  fetch('/api/profile', { headers: { Authorization: `Bearer ${accessToken}` } })
    .then((response) => (response.ok ? response.json() : null))
    .then((profile) => {
      if (!profile) return;
      const first = (profile.firstName || '').trim();
      const last = (profile.lastName || '').trim();
      const displayName = `${first} ${last}`.trim() || profile.username || 'Partner Account';
      if (headerName) headerName.textContent = displayName;
      if (headerAvatar) headerAvatar.textContent = (first[0] || displayName[0] || '?').toUpperCase();
      if (headerRank) headerRank.textContent = `Rank: ${profile.rank || 'Newbie'}`;
    })
    .catch(() => {});

  fetch('/api/messages/inbox', { headers: { Authorization: `Bearer ${accessToken}` } })
    .then((r) => (r.ok ? r.json() : []))
    .then((msgs) => {
      const unread = Array.isArray(msgs) ? msgs.filter((m) => !m.isRead).length : 0;
      if (headerNotifBadge) {
        if (unread > 0) {
          headerNotifBadge.textContent = String(unread);
          headerNotifBadge.style.display = 'grid';
        } else {
          headerNotifBadge.style.display = 'none';
        }
      }
    })
    .catch(() => {});
}

const headerShare = document.querySelector('#headerShare');
if (headerShare) {
  headerShare.addEventListener('click', () => {
    document.querySelector('#shareInvite')?.click();
  });
}

const sidebar = document.querySelector('aside, #sidebar');
const dashboardMenu = document.querySelector('#menu');
let navigationToggle = dashboardMenu;

const markActiveNavigation = () => {
  if (!sidebar) return;
  const currentPage = window.location.pathname.split('/').pop() || 'dashboard.html';
  const parentRoutes = {
    'activation.html': 'profile.html',
    'change-password.html': 'profile.html',
    'change-trans-password.html': 'profile.html',
    'reset-password.html': 'profile.html',
    'my-referrals.html': 'tree-view.html',
    'referral-tree.html': 'tree-view.html',
    'my-commission.html': 'my-income.html',
    'direct-referral-bonus.html': 'my-income.html',
    'level-bonus.html': 'my-income.html',
    'rank-bonus.html': 'my-income.html',
    'rank-history.html': 'my-income.html',
    'level-turnover.html': 'my-income.html',
    'monthly-salary.html': 'my-income.html',
    'purchase-list.html': 'shopping.html',
    'order-list.html': 'shopping.html',
    'withdraw-money.html': 'ewallet.html',
    'withdrawal-summary.html': 'ewallet.html',
    'cancelled-withdrawal.html': 'ewallet.html',
    'news.html': 'blog.html'
  };
  const parentPage = parentRoutes[currentPage];
  const activePages = new Set([currentPage, parentPage].filter(Boolean));
  sidebar.querySelectorAll('nav a[href]').forEach((link) => {
    const href = link.getAttribute('href');
    const isMessageTab = currentPage === 'messages.html' && href === `messages.html${window.location.hash}`;
    link.classList.toggle('active', activePages.has(href) || isMessageTab);
  });
  sidebar.querySelectorAll('.subnav').forEach((subnav) => {
    const parent = subnav.previousElementSibling;
    const hasActiveLink = Boolean(subnav.querySelector('a.active'));
    subnav.classList.toggle('portal-subnav-open', hasActiveLink);
    if (parent?.matches('a')) parent.setAttribute('aria-expanded', String(hasActiveLink));
  });
};

const initPortalNavigation = () => {
  markActiveNavigation();

  // Remove any stray fixed floating toggle if previously cached
  document.querySelectorAll('.portal-nav-toggle').forEach((el) => el.remove());

  const navToggle = document.querySelector('#portalNavToggleBtn') || document.querySelector('#menu');
  if (sidebar && navToggle) {
    const closeNav = () => {
      sidebar.classList.remove('portal-nav-open');
      document.body.classList.remove('portal-nav-active');
      navToggle.setAttribute('aria-expanded', 'false');
    };

    navToggle.addEventListener('click', (event) => {
      event.preventDefault();
      const isOpen = sidebar.classList.toggle('portal-nav-open');
      document.body.classList.toggle('portal-nav-active', isOpen);
      navToggle.setAttribute('aria-expanded', String(isOpen));
    });

    sidebar.querySelectorAll('a[href]').forEach((link) => link.addEventListener('click', closeNav));
    document.addEventListener('keydown', (event) => { if (event.key === 'Escape') closeNav(); });
  }
};

if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initPortalNavigation);
} else {
  initPortalNavigation();
}
window.addEventListener('hashchange', markActiveNavigation);

if (sidebar) {
  sidebar.querySelectorAll('.subnav').forEach((subnav) => {
    const parent = subnav.previousElementSibling;
    if (!parent?.matches('a')) return;
    parent.classList.add('portal-nav-parent');
    parent.addEventListener('click', (event) => {
      event.preventDefault();
      const shouldOpen = !subnav.classList.contains('portal-subnav-open');
      sidebar.querySelectorAll('.subnav').forEach((item) => {
        item.classList.remove('portal-subnav-open');
        const itemParent = item.previousElementSibling;
        if (itemParent?.matches('a')) itemParent.setAttribute('aria-expanded', 'false');
      });
      if (shouldOpen) {
        subnav.classList.add('portal-subnav-open');
        parent.setAttribute('aria-expanded', 'true');
      }
    });
  });
}

const shareInvite = document.querySelector('#shareInvite');
const shareInviteModal = document.querySelector('#shareInviteModal');
const shareInviteLink = document.querySelector('#shareInviteLink');
const shareInviteStatus = document.querySelector('#shareInviteStatus');
const shareScanner = document.querySelector('#shareScanner');
const shareCamera = document.querySelector('#shareCamera');
let shareCameraStream;
let shareScanFrame;

if (shareInvite && shareInviteModal && shareInviteLink) {
  const setInviteUrl = (username, name) => {
    const inviteUrl = new URL('vitality-registration.html', window.location.href);
    inviteUrl.searchParams.set('ref', username);
    inviteUrl.searchParams.set('recruiter', username);
    inviteUrl.searchParams.set('name', name);
    shareInviteLink.value = inviteUrl.href;
    const qr = document.querySelector('#shareInviteQr');
    if (qr) qr.src = `https://api.qrserver.com/v1/create-qr-code/?size=180x180&data=${encodeURIComponent(inviteUrl.href)}`;
  };
  setInviteUrl('samkelisojam', 'samkeliso ndlangamandla');
  const token = localStorage.getItem('vitality.accessToken');
  if (token) fetch('/api/profile', { headers: { Authorization: `Bearer ${token}` } }).then((response) => response.ok ? response.json() : null).then((profile) => {
    if (profile?.username) setInviteUrl(profile.username, `${profile.firstName || ''} ${profile.lastName || ''}`.trim() || 'Referral partner');
  }).catch(() => {});

  const stopScanner = () => {
    if (shareScanFrame) cancelAnimationFrame(shareScanFrame);
    shareScanFrame = undefined;
    shareCameraStream?.getTracks().forEach((track) => track.stop());
    shareCameraStream = undefined;
    if (shareCamera) shareCamera.srcObject = null;
  };

  const closeShareInvite = () => {
    stopScanner();
    shareInviteModal.classList.remove('is-open');
    shareInviteModal.setAttribute('aria-hidden', 'true');
  };

  const showShareStatus = (message) => { shareInviteStatus.textContent = message; };
  const openInviteLink = (value) => {
    try {
      const url = new URL(value);
      if (!url.pathname.endsWith('vitality-registration.html')) throw new Error();
      window.location.href = url.href;
    } catch { showShareStatus('Scan or paste a valid registration invite link.'); }
  };

  shareInvite.addEventListener('click', () => {
    shareInviteModal.classList.add('is-open');
    shareInviteModal.setAttribute('aria-hidden', 'false');
    document.querySelector('#closeShareInvite')?.focus();
  });
  document.querySelector('#closeShareInvite')?.addEventListener('click', closeShareInvite);
  shareInviteModal.addEventListener('click', (event) => { if (event.target === shareInviteModal) closeShareInvite(); });
  document.addEventListener('keydown', (event) => { if (event.key === 'Escape' && shareInviteModal.classList.contains('is-open')) closeShareInvite(); });

  document.querySelector('#copyInvite')?.addEventListener('click', async () => {
    try { await navigator.clipboard.writeText(shareInviteLink.value); showShareStatus('Invite link copied.'); }
    catch { shareInviteLink.select(); showShareStatus('Select the link and copy it manually.'); }
  });

  document.querySelector('#nativeShare')?.addEventListener('click', async () => {
    if (!navigator.share) { showShareStatus('Native sharing is not available in this browser. Use Copy link.'); return; }
    try { await navigator.share({ title: 'Join my ScaleEngine team', text: 'Register using my invite link.', url: shareInviteLink.value }); }
    catch (error) { if (error.name !== 'AbortError') showShareStatus('Unable to open sharing.'); }
  });

  document.querySelector('#scanInvite')?.addEventListener('click', async () => {
    shareScanner.classList.add('is-open');
    if (!navigator.mediaDevices?.getUserMedia || !('BarcodeDetector' in window)) {
      showShareStatus('Camera scanning is not supported here. Paste a link below.');
      return;
    }
    try {
      shareCameraStream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' }, audio: false });
      shareCamera.srcObject = shareCameraStream;
      await shareCamera.play();
      const detector = new BarcodeDetector({ formats: ['qr_code'] });
      const detect = async () => {
        if (!shareCameraStream) return;
        const codes = await detector.detect(shareCamera);
        if (codes[0]?.rawValue) { openInviteLink(codes[0].rawValue); return; }
        shareScanFrame = requestAnimationFrame(detect);
      };
      detect();
    } catch { showShareStatus('Camera access was unavailable. Paste a link below.'); }
  });
}