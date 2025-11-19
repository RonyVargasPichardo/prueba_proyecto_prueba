<template>
  <div class="login-page d-flex align-items-center justify-content-center min-vh-100">
    <div class="card login-card shadow-lg text-center animate__animated animate__fadeInDown">
      <div class="card-body px-5 py-4">
        <!-- Logo -->
        <img
          src="/src/assets/LogoContraloriaMobile.png"
          alt="Logo Contraloría"
          class="logo mb-2"
        />

        <h4 class="fw-bold mb-4 animate__animated animate__fadeInDown">
          <span class="titulo-login">Sorteo Navideño</span>
        </h4>

        <!-- Formulario -->
        <form @submit.prevent="login">
          <div class="mb-3 text-start">
            <label class="form-label">Usuario</label>
            <input
              v-model="usuario"
              type="text"
              class="form-control"
              placeholder="Ingrese su usuario"
              required
            />
          </div>
          <div class="mb-4 text-start">
            <label class="form-label">Contraseña</label>
            <input
              v-model="clave"
              type="password"
              class="form-control"
              placeholder="Ingrese su contraseña"
              required
            />
          </div>

          <button
            type="submit"
            class="btn btn-primary w-100 py-2 fw-bold"
            :disabled="cargando"
          >
            {{ cargando ? "Validando..." : "Iniciar sesión" }}
          </button>
        </form>

        <p class="mt-4 text-muted small">
          © 2025 Contraloría General de la República Dominicana
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/utilities/api";

export default {
  name: "LoginView",
  data() {
    return {
      usuario: "",
      clave: "",
      cargando: false,
    };
  },
  methods: {
    async login() {
      if (!this.usuario || !this.clave) return;

      this.cargando = true;

      try {
        const { data } = await api.post("/auth/login", {
          usuario: this.usuario,
          contrasena: this.clave,
        });

        if (data.success) {
          // Guardamos datos del usuario
          const usuarioData = data.data;

          localStorage.setItem("token", data.token);
          localStorage.setItem("perfil", usuarioData.rol === 1 ? "Administrador" : "Coordinador");
          localStorage.setItem("usuarioNombre", usuarioData.nombres);

          // ✅ Toast de bienvenida
          this.$toast?.add({
            severity: "success",
            summary: "Inicio de sesión correcto",
            detail: `Bienvenido, ${usuarioData.nombres}`,
            life: 2500,
          });

          // Redirigir al panel
          this.$router.push("/sorteos");
        } else {
          this.$toast?.add({
            severity: "warn",
            summary: "Acceso denegado",
            detail: data.message || "Credenciales incorrectas",
            life: 3000,
          });
        }
      } catch (error) {
        console.error("Error en el login:", error);
        this.$toast?.add({
          severity: "error",
          summary: "Error al iniciar sesión",
          detail: error.response?.data?.message || "No se pudo conectar al servidor",
          life: 3500,
        });
      } finally {
        this.cargando = false;
      }
    },
  },
};
</script>

<style scoped>
.login-page {
  width: 100%;
  background: transparent;
}

.login-card {
  max-width: 420px;
  border: none;
  border-radius: 1rem;
  background: rgba(255, 255, 255, 0.92);
  color: #222;
  backdrop-filter: blur(6px);
}

.titulo-login {
  color: #004c9a;
  font-weight: 700;
  letter-spacing: 0.5px;
  text-shadow: 0 1px 4px rgba(255, 255, 255, 0.5);
}

.logo {
  width: 180px;
  height: auto;
}

.btn-primary {
  background-color: #003870 !important;
  border: none;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background-color: #0055b3 !important;
  transform: scale(1.03);
}
</style>
