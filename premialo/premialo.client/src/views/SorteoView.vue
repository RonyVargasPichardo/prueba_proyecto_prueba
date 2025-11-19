<template>
  <div class="sorteo-view container-fluid py-2 animate__animated animate__fadeIn">
    <!-- 🔹 Header -->
    <Header :perfilUsuario="perfilUsuario" @cerrar-sesion="cerrarSesion" />

    <!-- 🔹 Contenido principal -->
    <main class="container mt-5 text-center">
      <!-- Si no hay sorteos -->
      <div v-if="sorteos.length === 0" class="sin-sorteo d-flex flex-column justify-content-center align-items-center">
        <div class="mensaje-principal animate__animated animate__pulse animate__infinite">
          <span>Crear nuevo sorteo</span>
        </div>
        <button class="btn btn-gradiente px-5 py-3 fw-semibold mt-4" @click="abrirModal">
          <i class="pi pi-plus-circle me-2"></i> Crear Sorteo
        </button>
      </div>

      <!-- Si hay sorteos -->
      <div v-else>
        <!--Buscador + Botón Nuevo sorteo -->
        <div class="acciones-bar mb-4 mt-3 px-3 ">
          <div class="search-box d-flex align-items-center">
            <i class="pi pi-search me-2 text-info"></i>
            <input v-model="filtro" @input="buscarSorteos" type="text" class="form-control filtro-input"
              placeholder="Buscar sorteo por nombre..." />
          </div>

          <button class="btn btn-gradiente px-4 py-2 fw-semibold" @click="abrirModal">
            <i class="pi pi-plus-circle me-2"></i> Nuevo sorteo
          </button>
        </div>


        <!-- Tarjetas de sorteos -->
        <div class="row g-4 justify-content-start px-4">
          <div v-for="(sorteo, index) in sorteosFiltrados" :key="index" class="col-12 col-md-6 col-lg-4">

            <div class="admin-card text-start p-4 animate__animated animate__fadeInUp"
              @click="abrirConfiguracionSorteo(sorteo)">
              <div class="d-flex justify-content-end align-items-center mb-3">
                <span class="fecha-sorteo">{{ formatearFecha(sorteo.fechaSorteo) }}</span>
              </div>
              <div class="d-flex justify-content-start align-items-center mb-3">
                <h5 class="fw-bold titulo-navidad mb-0">
                  <!-- <i class="pi pi-sparkles me-2 text-info"></i> -->
                  {{ sorteo.nombre }}
                </h5>

              </div>

              <p class="text-light small mb-4">{{ sorteo.descripcion }}</p>

              <!-- Datos: premios / participantes -->
              <div class="datos-sorteo d-flex justify-content-around align-items-center pt-3 ">
                <div class="d-flex align-items-center gap-2">
                  <i class="pi pi-gift text-success fs-5"></i>
                  <div>
                    <span class="fw-semibold text-white-50 d-block">Premios</span>
                    <span class="fw-bold text-success">{{ sorteo.premios }}</span>
                  </div>
                </div>

                <div class="division"></div>

                <div class="d-flex align-items-center gap-2">
                  <i class="pi pi-users text-info fs-5"></i>
                  <div>
                    <span class="fw-semibold text-white-50 d-block">Participantes</span>
                    <span class="fw-bold text-info">{{ sorteo.participantes }}</span>
                  </div>
                </div>
              </div>

              <!-- 🔹 Iconos de acción -->
              <div class="acciones mt-3 d-flex justify-content-end gap-4">
                <i class="pi pi-pencil accion-icono" title="Editar" @click.stop="editarSorteo(index)"></i>
                <i class="pi pi-trash accion-icono" title="Eliminar" @click.stop="eliminarSorteo(index)"></i>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- 🔹 Modal Crear/Editar Sorteo -->
    <transition name="fade">
      <div v-if="mostrarModal" class="modal-overlay">
        <div class="modal-content shadow-lg animate__animated animate__zoomIn">
          <h4 class="fw-bold text-primary mb-3">
            <i class="pi pi-gift me-2"></i>
            {{ editando ? "Editar Sorteo" : "Nuevo Sorteo" }}
          </h4>

          <form @submit.prevent="guardarSorteo">
            <div class="mb-3 text-start">
              <label class="form-label fw-semibold">Nombre del sorteo</label>
              <input type="text" v-model="nuevoSorteo.nombre" class="form-control" required />
            </div>

            <div class="mb-3 text-start">
              <label class="form-label fw-semibold">Descripción</label>
              <textarea v-model="nuevoSorteo.descripcion" class="form-control" rows="3" required></textarea>
            </div>

            <div class="mb-4 text-start">
              <label class="form-label fw-semibold">Fecha del sorteo</label>
              <input type="date" v-model="nuevoSorteo.fecha" class="form-control" required />
            </div>

            <div class="d-flex justify-content-end gap-2">
              <button type="button" class="btn btn-gradiente-cancelar btn-outline-secondary" @click="cerrarModal">
                Cancelar
              </button>
              <button type="submit" class="btn btn-gradiente btn-margin px-4">
                {{ editando ? "Actualizar" : "Guardar" }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </transition>

    <!-- 🔹 Modal Confirmar Eliminación -->
    <transition name="fade">
      <div v-if="mostrarConfirmacion" class="modal-overlay">
        <div class="modal-content shadow-lg animate__animated animate__fadeInUp text-center">
          <i class="pi pi-exclamation-triangle text-warning fs-1 mb-3"></i>
          <h5 class="fw-bold mb-3 text-dark">
            ¿Eliminar sorteo?
          </h5>
          <p class="text-secondary mb-4">
            Estás a punto de eliminar <strong>{{ sorteoAEliminar?.nombre }}</strong>.<br />
            Esta acción no se puede deshacer.
          </p>

          <div class="d-flex justify-content-center gap-3">
            <button class="btn btn-gradiente-cancelar px-4" @click="cancelarEliminacion">
              Cancelar
            </button>
            <button class="btn btn-gradiente btn-margin px-4" @click="confirmarEliminacion">
              Eliminar
            </button>
          </div>
        </div>
      </div>
    </transition>

  </div>
</template>

<script>
import Header from "@/components/Header.vue";
import api from "@/utilities/api";
import _ from "lodash";
import { useToast } from "primevue/usetoast";

export default {
  name: "SorteoView",
  components: { Header },
  data() {
    return {
      toast: useToast(),
      perfilUsuario: localStorage.getItem("perfil") || "Administrador",
      sorteos: [],
      mostrarModal: false,
      mostrarConfirmacion: false,
      sorteoAEliminar: null,
      editando: false,
      filtro: "",
      indiceEditando: null,
      nuevoSorteo: {
        id: null,
        nombre: "",
        descripcion: "",
        fecha: "",
        premios: 0,
        participantes: 0,
      },
    };
  },
  computed: {
    sorteosFiltrados() {
      const texto = this.filtro.toLowerCase();
      return this.sorteos.filter(s => s.nombre.toLowerCase().includes(texto));
    },
  },
  methods: {
    abrirModal() {
      this.mostrarModal = true;
    },
    cerrarModal() {
      this.mostrarModal = false;
      this.editando = false;
      this.indiceEditando = null;
      this.nuevoSorteo = {
        id: null,
        nombre: "",
        descripcion: "",
        fecha: "",
        premios: 0,
        participantes: 0,
      };
    },
    async guardarSorteo() {
      if (!this.nuevoSorteo.nombre.trim() || !this.nuevoSorteo.fecha) {
        this.$toast.add({
          severity: "warn",
          summary: "Campos requeridos",
          detail: "Debes completar el nombre y la fecha del sorteo.",
          life: 3500,
        });
        return;
      }

      const payload = {
        Id: this.nuevoSorteo.id,
        Nombre: this.nuevoSorteo.nombre,
        Descripcion: this.nuevoSorteo.descripcion,
        FechaSorteo: this.nuevoSorteo.fecha,
      };

      try {
        if (this.editando && this.nuevoSorteo.id) {
          // 🔹 PUT - editar sorteo existente
          await api.put(`/sorteos/${this.nuevoSorteo.id}`, payload);

          this.$toast.add({
            severity: "success",
            summary: "Sorteo actualizado",
            detail: `El sorteo "${this.nuevoSorteo.nombre}" se actualizó correctamente.`,
            life: 3000,
          });
        } else {
          // 🔹 POST - crear nuevo sorteo
          await api.post("/sorteos", payload);

          this.$toast.add({
            severity: "success",
            summary: "Sorteo creado",
            detail: `El sorteo "${this.nuevoSorteo.nombre}" se ha registrado correctamente.`,
            life: 3000,
          });
        }

        await this.cargarSorteos(); // refresca lista
        this.cerrarModal();
      } catch (error) {
        console.error("Error al guardar sorteo:", error);

        this.$toast.add({
          severity: "error",
          summary: "Error al guardar",
          detail: "Ocurrió un problema al guardar el sorteo. Verifica la conexión o autorización.",
          life: 4000,
        });
      }
    },
    editarSorteo(index) {
      const sorteo = this.sorteos[index];
      this.nuevoSorteo = {
        id: sorteo.id,
        nombre: sorteo.nombre,
        descripcion: sorteo.descripcion,
        fecha: sorteo.fechaSorteo,
        premios: sorteo.premios,
        participantes: sorteo.participantes,
      };
      this.editando = true;
      this.indiceEditando = index;
      this.mostrarModal = true;
    },
    eliminarSorteo(index) {
      this.sorteoAEliminar = this.sorteos[index];
      this.mostrarConfirmacion = true;
    },
    cancelarEliminacion() {
      this.mostrarConfirmacion = false;
      this.sorteoAEliminar = null;
    },
    async confirmarEliminacion() {
      if (!this.sorteoAEliminar?.id) return;

      try {
        await api.delete(`/sorteos/${this.sorteoAEliminar.id}`);

        this.mostrarConfirmacion = false;
        this.sorteoAEliminar = null;

        // 🔹 Toast de éxito
        this.$toast.add({
          severity: "success",
          summary: "Sorteo eliminado",
          detail: "El sorteo fue eliminado correctamente.",
          life: 3000,
        });

        await this.cargarSorteos();
      } catch (error) {
        console.error("Error al eliminar sorteo:", error);

        this.mostrarConfirmacion = false;

        // 🔹 Toast de error
        this.$toast.add({
          severity: "error",
          summary: "Error al eliminar",
          detail: "No se pudo eliminar el sorteo. Verifica tu autorización.",
          life: 4000,
        });
      }
    },
    cerrarSesion() {
      localStorage.removeItem("perfil");
      this.$router.push("/login");
    },
    abrirConfiguracionSorteo(sorteo) {
      this.$router.push(`/sorteos/${sorteo.id}`);
    },
    formatearFecha(fecha) {
      if (!fecha) return "";
      const f = new Date(fecha);
      if (isNaN(f)) return "Fecha inválida";
      return f.toLocaleDateString("es-DO", { year: "numeric", month: "long", day: "numeric" });
    },
    async cargarSorteos(nombre = "") {
      try {
        const response = await api.get("/sorteos", {
          params: { nombre: nombre || "" },
        });
        this.sorteos = response.data || [];
      } catch (error) {
        console.error("Error al cargar sorteos:", error);
      }
    },
    buscarSorteos: _.debounce(function () {
      this.cargarSorteos(this.filtro);
    }, 400),
  },
  mounted() {
    this.cargarSorteos();
  },
};
</script>

<style scoped>
.sorteo-view {
  min-height: 100vh;
  color: #fff;
}

.sin-sorteo {
  height: 75vh;
}

.mensaje-principal {
  font-size: 4rem;
  font-weight: 800;
  color: #b6dbff;
  text-shadow: 0 0 20px rgba(255, 255, 255, 0.45),
    0 0 35px rgba(90, 180, 255, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  letter-spacing: 1px;
}

.row.g-4 {
  justify-content: flex-start !important;
  padding-left: 3rem;
  padding-right: 3rem;
}

/* 🔹 Tarjeta */
.admin-card {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px);
  border-radius: 1rem;
  box-shadow: 0 4px 14px rgba(0, 0, 0, 0.25);
  transition: all 0.3s ease;
  color: #e6f0ff;
  cursor: pointer;
  width: 380px;
  padding: 2rem;
  margin-bottom: 2rem;
}

.admin-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.35);
}

/* 🔹 Contenedor alineado horizontalmente */
.acciones-bar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 1rem;
  flex-wrap: wrap;
}

/* 🔹 Input de búsqueda tipo glass elegante */
.search-box {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(12px);
  border-radius: 0.75rem;
  padding: 0.45rem 0.9rem;
  width: 270px;
  transition: all 0.3s ease;
}

.filtro-input {
  background: transparent;
  border: none;
  color: #e6f0ff;
  font-size: 0.95rem;
  outline: none;
  width: 100%;
}

.filtro-input::placeholder {
  color: rgba(230, 240, 255, 0.6);
}

.search-box:focus-within {
  box-shadow: 0 0 10px rgba(0, 180, 255, 0.4);
  transform: translateY(-1px);
}

/* 🔹 Título con ícono fijo */
.titulo-navidad {
  font-size: 1.4rem;
  font-weight: 800;
  color: #aefcff;
  display: flex;
  align-items: center;
  letter-spacing: 0.5px;
}

.pi-sparkles {
  font-size: 1.1rem;
  opacity: 0.9;
}

/* 🔹 Datos inferiores */
.datos-sorteo {
  text-align: center;
  border-top: 1px solid rgba(255, 255, 255, 0.2);
}

.division {
  width: 2px;
  height: 40px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 2px;
}

/* 🔹 Iconos de acción */
.acciones .accion-icono {
  font-size: 1rem;
  color: #7fc8ff;
  cursor: pointer;
  transition: 0.3s ease;
}

.acciones .accion-icono:hover {
  color: #b8e0ff;
  transform: scale(1.25);
}

/* 🔹 Botón degradado institucional */
.btn-gradiente {
  background: linear-gradient(135deg, #0072ff, #0051c8);
  border: none;
  color: #fff !important;
  border-radius: 0.6rem;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 85, 255, 0.3);
  /* margin-top: 2rem; */
  margin-right: 2rem;
}

.btn-margin {
  margin-top: 2rem;
}

.btn-gradiente:hover {
  transform: translateY(-2px);
  background: linear-gradient(135deg, #3399ff, #0066ff);
  box-shadow: 0 6px 18px rgba(0, 100, 255, 0.4);
}


/* 🔹 Botón degradado institucional */
.btn-gradiente-cancelar {
  background: linear-gradient(135deg, #ca2931, #da5a6b);
  border: none;
  color: #fff !important;
  border-radius: 0.6rem;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 85, 255, 0.3);
  margin-top: 2rem;
}

.btn-gradiente-cancelar:hover {
  transform: translateY(-2px);
  background: linear-gradient(135deg, #d11620c9, #e03148e8);
  box-shadow: 0 6px 18px rgba(255, 0, 0, 0.158);
}


/* 🔹 Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(15, 23, 42, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.modal-content {
  background: #fff;
  border-radius: 1rem;
  padding: 2rem;
  max-width: 500px;
  width: 90%;
  color: #1e293b;
}

/* 🔹 Animaciones */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.4s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* 🔹 Modal confirmación */
.modal-content.text-center {
  background: #ffffff;
  border-radius: 1rem;
  padding: 2rem 2.5rem;
  color: #1e293b;
  max-width: 420px;
  width: 90%;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
  animation: zoomIn 0.3s ease;
}

.modal-content .pi-exclamation-triangle {
  color: #facc15;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 3000;
}

@keyframes zoomIn {
  from {
    transform: scale(0.8);
    opacity: 0;
  }

  to {
    transform: scale(1);
    opacity: 1;
  }
}
</style>
