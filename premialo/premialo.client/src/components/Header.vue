<template>
  <header
    class="admin-header d-flex align-items-center justify-content-between px-4 py-3 shadow-sm animate__animated animate__fadeInDown">
    <div class="d-flex align-items-center gap-3">
      <div class="logo-bg">
        <img src="/src/assets/LogoContraloriaMobile.png" alt="Logo Contraloría" class="logo-header" />
      </div>
      <!-- 🔹 Título dinámico -->
      <h1 class="mb-0 text-white fw-bold fs-4">
        Panel de {{ perfilUsuario }}
      </h1>
    </div>

    <!-- 🔹 Menú de usuario -->
    <div class="user-menu position-relative">
      <button class="user-chip fw-semibold d-flex align-items-center gap-2" @click="toggleMenu">
        <i class="bi bi-person-circle fs-5"></i>
        <span>{{ perfilUsuario }}</span>
      </button>

      <transition name="fade-slide">
        <div v-if="menuVisible" class="dropdown-menu-custom animate__animated animate__fadeInDown">
          <button class="dropdown-item-custom text-danger" @click="$emit('cerrar-sesion')">
            <i class="bi bi-box-arrow-right me-2"></i> Cerrar sesión
          </button>
        </div>
      </transition>
    </div>
  </header>
</template>

<script>
export default {
  name: "AdminHeader",
  props: {
    perfilUsuario: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      menuVisible: false,
    };
  },
  methods: {
    toggleMenu() {
      this.menuVisible = !this.menuVisible;
    },
  },
};
</script>

<style scoped>
.admin-header {
  position: sticky;
  top: 0;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(8px);
  border-radius: 0.75rem;
  z-index: 100;
}

.logo-bg {
  background: rgba(255, 255, 255, 0.85);
  border-radius: 1rem;
  padding: 0.4rem 0.8rem;
}

.logo-header {
  width: 80px;
  height: auto;
}

.user-chip {
  background: linear-gradient(135deg, #0058b0, #003b84);
  color: #fff;
  border: none;
  padding: 0.55rem 1.3rem;
  border-radius: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 0 10px rgba(0, 91, 196, 0.4);
}

.user-chip:hover {
  background: linear-gradient(135deg, #0066cc, #004a99);
  box-shadow: 0 0 18px rgba(0, 123, 255, 0.7);
  transform: translateY(-1px);
}

.dropdown-menu-custom {
  position: absolute;
  right: 0;
  top: 115%;
  background: rgba(0, 60, 130, 0.35);
  backdrop-filter: blur(12px);
  border-radius: 0.75rem;
  padding: 0.4rem;
  min-width: 160px;
  z-index: 200;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.25);
}

.dropdown-item-custom {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  width: 100%;
  color: #ffffff;
  background: transparent;
  border: none;
  padding: 0.6rem 0.8rem;
  border-radius: 0.5rem;
  text-align: center;
  font-weight: 500;
  transition: all 0.25s ease;
}

.dropdown-item-custom:hover {
  background: rgba(255, 255, 255, 0.15);
  color: #ff4b4b;
}

.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(-8px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
