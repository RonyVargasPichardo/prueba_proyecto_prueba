<template>
  <aside class="sidebar-admin">
    <!-- LOGO / HEADER DEL SIDEBAR -->
    <div class="sidebar-header">
      <i class="pi pi-shield text-white me-2"></i>
      <span class="titulo">Administración</span>
    </div>

    <!-- OPCIONES -->
    <ul class="menu-list">
      <li
        :class="['menu-item', isActive('/admin/usuarios') && 'active']"
        @click="go('/admin/usuarios')"
      >
        <i class="pi pi-users me-2"></i>
        <span>Usuarios</span>
      </li>

      <li
        :class="['menu-item', isActive('/admin/registrar') && 'active']"
        @click="go('/admin/registrar')"
      >
        <i class="pi pi-user-plus me-2"></i>
        <span>Registrar Usuario</span>
      </li>

      <!-- AGREGAR MÁS OPCIONES FUTURAS -->
      <!-- 
      <li
        :class="['menu-item', isActive('/admin/roles') && 'active']"
        @click="go('/admin/roles')"
      >
        <i class="pi pi-lock me-2"></i>
        <span>Roles</span>
      </li>
      -->
    </ul>
  </aside>
</template>

<script setup>
import { useRouter, useRoute } from "vue-router";

const router = useRouter();
const route = useRoute();

// Ir a una ruta
const go = (ruta) => {
  router.push(ruta);
};

// Determinar el botón activo
const isActive = (path) => {
  return route.path.startsWith(path);
};
</script>

<style scoped>
.sidebar-admin {
  width: 240px;
  height: 100vh;
  background-color: #003870; /* Azul institucional */
  color: white;
  padding-top: 20px;
  display: flex;
  flex-direction: column;
  box-shadow: 0 0 12px rgba(0,0,0,0.15);
  position: fixed;
  left: 0;
  top: 0;
  z-index: 20;
}

.sidebar-header {
  padding: 0 20px 20px;
  font-size: 1.2rem;
  font-weight: bold;
  display: flex;
  align-items: center;
  letter-spacing: .5px;
}

.menu-list {
  list-style: none;
  padding: 0;
  margin: 0;
  flex-direction: column;
}

.menu-item {
  padding: 12px 20px;
  font-size: .95rem;
  display: flex;
  align-items: center;
  cursor: pointer;
  transition: .2s ease;
  border-left: 4px solid transparent;
}

.menu-item:hover {
  background-color: rgba(255,255,255,0.14);
}

.menu-item.active {
  background-color: rgba(255,255,255,0.25);
  border-left: 4px solid #ffd54f; /* Línea amarilla elegante */
  font-weight: 600;
}
</style>
