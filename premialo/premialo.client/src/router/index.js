import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'


const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView },
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
    name: 'RegistrarUsuarios',
    component: () => import('../views/admin/RegistrarUsuarios.vue')
  },
  {
    path: '/sorteos',
    name: 'SorteoView',
    component: () => import('../views/SorteoView.vue')
  },
  {
    path: '/sorteos/:id',
    name: 'AdminView',
    component: () => import('../views/AdminView.vue')
  },
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
