import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView, meta: { public: true } },

  // ======================================================
  // 🟦 SECCIÓN DE ADMINISTRACIÓN
  // ======================================================
  {
    path: '/admin',
    component: () => import('@/views/admin/AdminLayoutView.vue'),
    meta: { roles: ['Admin'] },

    children: [
      {
        path: 'usuarios',
        name: 'AdminUsuarios',
        component: () => import('@/views/admin/ListadoUsuarios.vue'),
        meta: { roles: ['Admin'] }
      },
      {
        path: 'registrar',
        name: 'AdminRegistrarUsuario',
        component: () => import('@/views/admin/RegistrarUsuarios.vue'),
        meta: { roles: ['Admin'] }
      },
      {
        path: 'editar/:idUsuario',
        name: 'AdminEditarUsuario',
        component: () => import('@/views/admin/RegistrarUsuarios.vue'),
        meta: { roles: ['Admin'] }
      },
    ]
  },

  // ======================================================
  // 🟨 LISTADO GENERAL DE SORTEOS (coordinador)
  // ======================================================
  {
    path: '/sorteos',
    name: 'SorteoView',
    component: () => import('@/views/SorteoView.vue'),
    meta: { roles: ['Coordinador'] }
  },

  // ======================================================
  // 🟨 LAYOUT DEL SORTEO > subrutas internas
  // ======================================================
  {
    path: '/sorteos/:id',
    component: () => import('@/views/SorteoLayoutView.vue'),
    meta: { roles: ['Coordinador'] },

    children: [
      {
        path: 'configuracion',
        name: 'ConfiguracionView',
        component: () => import('@/views/ConfiguracionView.vue'),
        meta: { roles: ['Coordinador'] }
      },
      {
        path: 'asistencia',
        name: 'AsistenciaView',
        component: () => import('@/views/AsistenciaView.vue'),
        meta: { roles: ['Coordinador'] }
      },
      {
        path: 'participantes',
        name: 'ParticipantesView',
        component: () => import('@/views/ParticipantesView.vue'),
        meta: { roles: ['Coordinador'] }
      },
      {
        path: 'premios',
        name: 'PremiosView',
        component: () => import('@/views/PremiosView.vue'),
        meta: { roles: ['Coordinador'] }
      },
      {
        path: 'resultados',
        name: 'ResultadosView',
        component: () => import('@/views/ResultadosView.vue'),
        meta: { roles: ['Coordinador'] }
      },
    ]
  },

  // ======================================================
  // 🟥 OTRAS VISTAS ESPECIALES (solo coordinador)
  // ======================================================
  {
    path: '/TragaColaboradores',
    name: 'TragaMoneadas',
    component: () => import('@/views/TragaMoneadas.vue'),
    meta: { roles: ['Coordinador'] }
  },
  {
    path: '/Iniciosorteo',
    name: 'Iniciosorteo',
    component: () => import('@/views/Iniciosorteo.vue'),
    meta: { roles: ['Coordinador'] }
  },
  {
    path: '/sortear/:id',
    name: 'Sortear',
    component: () => import('@/views/Sortear.vue'),
    meta: { roles: ['Coordinador'] }
  },
  {
    path: '/403',
    name: 'AccesoDenegado',
    component: () => import('@/views/Error403.vue'),
    meta: { public: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// =========================
// 🔥 GUARD GLOBAL DEL ROUTER
// =========================
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");
  const rol = localStorage.getItem("rol");

  // 1️⃣ Rutas públicas (login)
  if (to.meta.public) {
    return next();
  }

  // 2️⃣ Si intenta entrar a cualquier ruta protegida sin token → login
  if (!token) {
    return next("/login");
  }

  // 3️⃣ Si la ruta requiere roles específicos
  if (to.meta.roles && to.meta.roles.length > 0) {
    if (!rol || !to.meta.roles.includes(rol)) {
      return next("/403");  // puedes crear esta vista
    }
  }

  // 4️⃣ Todo ok → continuar
  next();
});

export default router
