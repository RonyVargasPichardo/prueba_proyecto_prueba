import axios from "axios";
import { jwtDecode } from "jwt-decode";

// 🔹 Usa tu variable de entorno o coloca directamente la URL base
const baseURL = import.meta.env.VITE_API_URL;



const api = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

// ============================================================
// 🔹 Interceptor de REQUEST
// ============================================================
// Inserta automáticamente el token en los headers
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;

      // Opcional: verificar expiración del JWT
      try {
        const decoded = jwtDecode(token);
        const exp = decoded.exp * 1000;
        if (Date.now() >= exp) {
          console.warn("⏰ Token expirado, redirigiendo al login...");
          localStorage.removeItem("token");
          localStorage.removeItem("perfil");
          localStorage.removeItem("usuarioNombre");
          window.location.href = "/login";
          throw new Error("Token expirado");
        }
      } catch (e) {
        console.error("Error al decodificar token:", e);
      }
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// ============================================================
// 🔹 Interceptor de RESPONSE
// ============================================================
// Maneja errores comunes globalmente
api.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error.response?.status;

    if (status === 401) {
      console.warn("🔒 Sesión no autorizada o expirada.");
      localStorage.removeItem("token");
      localStorage.removeItem("perfil");
      localStorage.removeItem("usuarioNombre");
      window.location.href = "/login";
    } else if (status === 500) {
      console.error("💥 Error interno del servidor:", error.response.data);
    }

    return Promise.reject(error);
  }
);

export function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("perfil");
  localStorage.removeItem("usuarioNombre");
  window.location.href = "/login";
}
export default api;
