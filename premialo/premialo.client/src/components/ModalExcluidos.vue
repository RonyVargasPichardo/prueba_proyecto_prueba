<template>
  <transition name="fade-overlay">
    <div v-if="visible" class="overlay-opaco"></div>
  </transition>

  <transition name="fade-modal">
    <div v-if="visible" class="modal-container">
      <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content moderno-modal shadow-xl border-0">
          <!-- HEADER -->
          <div class="modal-header header-roja">
            <div
              class="d-flex align-items-center gap-2 w-100 justify-content-between"
            >
              <h5 class="modal-title fw-bold text-white mb-0">
                <i class="pi pi-user-minus me-2"></i> Lista de Excluidos
              </h5>

              <!-- Filtro -->
              <div
                class="filtro-mini d-flex align-items-center bg-white px-2 rounded-3"
              >
                <i class="pi pi-search text-danger small"></i>
                <input
                  type="text"
                  v-model="filtroCedula"
                  class="filtro-mini-input"
                  placeholder="Cédula..."
                />
              </div>
            </div>
            <button
              class="btn-close btn-close-white ms-2"
              @click="$emit('cerrar')"
            ></button>
          </div>

          <!-- BODY -->
          <div class="modal-body px-4 py-3">
            <div class="table-responsive table-wrapper">
              <table class="table tabla-compacta align-middle">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Nombre</th>
                    <th>Cédula</th>
                    <th>Departamento</th>
                    <th>Cargo</th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="(e, index) in excluidosFiltrados"
                    :key="index"
                    class="text-danger fw-semibold"
                  >
                    <td>{{ index + 1 }}</td>
                    <td>{{ e.Nombre || e.nombre }}</td>
                    <td>{{ e.Cédula || e.cedula }}</td>
                    <td>{{ e.Departamento || e.departamento }}</td>
                    <td>{{ e.Cargo || e.cargo }}</td>
                  </tr>

                  <tr v-if="excluidosFiltrados.length === 0">
                    <td colspan="5" class="text-center text-muted py-3">
                      <i class="pi pi-info-circle me-2"></i> No hay excluidos
                      cargados.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- FOOTER -->
          <div class="modal-footer justify-content-center border-0 pb-4">
            <button
              class="btn btn-gradiente-rojo px-4 fw-semibold"
              @click="$emit('cerrar')"
            >
              <i class="pi pi-times me-2"></i> Cerrar
            </button>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
export default {
  name: "ModalExcluidos",
  props: {
    visible: Boolean,
    excluidos: Array,
  },
  data() {
    return {
      filtroCedula: "",
    };
  },
  computed: {
    excluidosFiltrados() {
      if (!this.filtroCedula.trim()) return this.excluidos;
      const texto = this.filtroCedula.toLowerCase();
      return this.excluidos.filter((e) =>
        (e.cedula || e.Cédula).toLowerCase().includes(texto)
      );
    },
  },
  watch: {
    visible(val) {
      document.body.style.overflow = val ? "hidden" : "auto";
    },
  },
};
</script>

<style scoped>
/* === 🔹 Input mini === */
.filtro-mini {
  position: relative;
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(6px);
  border: 1px solid rgba(176, 0, 0, 0.2);
  border-radius: 0.6rem;
  height: 34px;
  min-width: 170px;
  padding: 0 0.75rem;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  transition: all 0.25s ease-in-out;
}

.filtro-mini i {
  font-size: 0.9rem;
  color: #b91c1c;
  opacity: 0.75;
}

.filtro-mini-input {
  border: none;
  outline: none;
  background: transparent;
  font-size: 0.85rem;
  color: #1e293b;
  width: 100%;
  margin-left: 0.5rem;
  font-weight: 500;
}

/* === 🔹 Overlay y contenedor === */
.overlay-opaco {
  position: fixed;
  top: 5;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100%;
  height: 100%;
  background: rgba(15, 23, 42, 0.65);
  z-index: 2000;
}

.modal-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2001;
  overflow: hidden;
}

/* === 🔹 Modal === */
.moderno-modal {
  background: #f9fafc;
  color: #1e293b;
  border-radius: 1rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
  animation: scaleIn 0.35s ease forwards;
  max-height: 90vh;
  overflow-y: auto;
}

/* === 🔹 Header rojo === */
.header-roja {
  background: linear-gradient(90deg, #9b1c1c, #d43b3b);
  color: white;
  padding: 1rem 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: sticky;
  top: 0;
  z-index: 10;
}

/* === 🔹 Botón gradiente rojo === */
.btn-gradiente-rojo {
  background: linear-gradient(90deg, #9b1c1c, #d43b3b);
  color: white;
  border: none;
  border-radius: 0.6rem;
  padding: 0.6rem 1.3rem;
  transition: all 0.3s ease;
}
.btn-gradiente-rojo:hover {
  transform: scale(1.04);
  box-shadow: 0 0 10px rgba(212, 59, 59, 0.4);
}

/* === 🔹 Tabla === */
.table-wrapper {
  max-height: 60vh;
  min-height: 300px;
  overflow-y: auto;
  background-color: white;
  border-radius: 0.6rem;
  box-shadow: inset 0 0 8px rgba(0, 0, 0, 0.05);
  transition: all 0.25s ease;
}

.tabla-compacta {
  width: 100%;
  border-collapse: collapse;
}

.tabla-compacta thead {
  background-color: #9b1c1c;
  color: #fff;
  position: sticky;
  top: 0;
  z-index: 5;
}

/* === 🔹 Transiciones === */
@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.fade-overlay-enter-active,
.fade-overlay-leave-active {
  transition: opacity 0.3s ease;
}
.fade-overlay-enter-from,
.fade-overlay-leave-to {
  opacity: 0;
}

.fade-modal-enter-active,
.fade-modal-leave-active {
  transition: opacity 0.3s ease, transform 0.35s ease;
}
.fade-modal-enter-from,
.fade-modal-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* === 🔹 Tamaño del modal === */
.modal-dialog.modal-xl {
  max-width: 1200px;
  width: 95%;
  margin: 0 !important;
}
</style>
