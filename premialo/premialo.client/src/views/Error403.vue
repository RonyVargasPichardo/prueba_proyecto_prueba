<template>
  <div class="error-wrapper animate__animated animate__fadeIn">
    <div class="error-card">
      <div class="icon-container">
        <i class="bi bi-shield-lock-fill lock-icon"></i>
      </div>

      <h1 class="error-title">403</h1>
      <h2 class="error-subtitle">Acceso Denegado</h2>

      <p class="error-text">
        No tienes permisos para acceder a esta sección.
        <br />
        Si crees que se trata de un error, por favor contacta al administrador.
      </p>

      <button class="back-btn" @click="volver">
        <i class="bi bi-arrow-left-short"></i>
        Regresar
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: "Error403",
  methods: {
    volver() {
      // Si viene del admin, vuelve al admin
      const rol = localStorage.getItem("rol");

      if (rol === "Admin") {
        return this.$router.push("/admin/usuarios");
      }

      // Si viene del coordinador
      return this.$router.push("/sorteos");
    },
  },
};
</script>

<style scoped>
/* Fondo completo con blur */
.error-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;

  background: linear-gradient(135deg, #003870 0%, #001e44 100%);
  color: white;
}

/* Tarjeta principal */
.error-card {
  text-align: center;
  padding: 3rem 3.5rem;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border-radius: 1.3rem;
  max-width: 460px;
  box-shadow: 0 6px 30px rgba(0, 0, 0, 0.25);
  animation-duration: 0.8s;
}

/* Icono */
.icon-container {
  background: rgba(255, 255, 255, 0.15);
  width: 82px;
  height: 82px;
  border-radius: 50%;
  margin: 0 auto 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.lock-icon {
  font-size: 2.8rem;
  color: #ffd056;
}

/* Textos */
.error-title {
  font-size: 4rem;
  font-weight: 800;
  margin: 0;
}

.error-subtitle {
  font-size: 1.4rem;
  font-weight: 600;
  margin-top: -0.4rem;
  color: #dfe7f1;
}

.error-text {
  margin: 1rem 0 2rem;
  font-size: 1rem;
  opacity: 0.85;
  line-height: 1.6;
}

/* Botón */
.back-btn {
  background: linear-gradient(135deg, #0058b0, #003b84);
  border: none;
  color: white;
  padding: 0.7rem 1.7rem;
  border-radius: 1.7rem;
  font-weight: 600;
  cursor: pointer;
  font-size: 1rem;
  transition: 0.25s ease;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.back-btn:hover {
  background: linear-gradient(135deg, #0066cc, #004a99);
  box-shadow: 0 0 14px rgba(0, 91, 196, 0.45);
  transform: translateY(-2px);
}
</style>
