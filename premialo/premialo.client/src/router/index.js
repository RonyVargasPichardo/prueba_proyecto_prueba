import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView },

  // 🔹 Administración de usuarios
  {
    path: '/Listadousuarios',
    name: 'ListadoUsuarios',
    component: () => import('../views/admin/ListadoUsuarios.vue')
  },
  {
    path: '/ConfiguracionUsuarios',
    name: 'RegistrarUsuarios',
    component: () => import('../views/admin/RegistrarUsuarios.vue')
  },
  {
    path: '/ConfiguracionUsuarios/:idUsuario',
    name: 'EditarUsuarios',
    component: () => import('../views/admin/RegistrarUsuarios.vue')
  },

  // 🔹 Vista general de sorteos
  {
    path: '/sorteos',
    name: 'SorteoView',
    component: () => import('../views/SorteoView.vue')
  },

  // 🔹 Layout principal del sorteo con sus subrutas
  {
    path: '/sorteos/:id',
    component: () => import('@/views/SorteoLayoutView.vue'),
    children: [
      {
        path: 'configuracion',
        name: 'ConfiguracionView',
        component: () => import('@/views/ConfiguracionView.vue'),
      },
      {
        path: 'asistencia',
        name: 'AsistenciaView',
        component: () => import('@/views/AsistenciaView.vue'),
      },
      {
        path: 'participantes',
        name: 'ParticipantesView',
        component: () => import('@/views/ParticipantesView.vue'),
      },
      {
        path: 'premios',
        name: 'PremiosView',
        component: () => import('@/views/PremiosView.vue'),
      },
      {
        path: 'resultados',
        name: 'ResultadosView',
        component: () => import('@/views/ResultadosView.vue'),
      },
    ]
  },

  // 🔹 Otras vistas especiales
  {
    path: '/TragaColaboradores',
    name: 'TragaMoneadas',
    component: () => import('../views/TragaMoneadas.vue')
  },
  {
    path: '/Iniciosorteo',
    name: 'Iniciosorteo',
    component: () => import('../views/Iniciosorteo.vue')
  },
  {
    path: '/sortear/:id',
    name: 'Sortear',
    component: () => import('../views/Sortear.vue')
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
