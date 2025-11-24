<template>
  <div class="premios-view animate__animated animate__fadeIn">
    <!-- HEADER -->
    <div class="header-gradiente d-flex align-items-center justify-content-between mb-3">
      <h4 class="fw-bold text-white mb-0">
        <i class="pi pi-gift me-2"></i> Lista de Premios
      </h4>
    </div>

    <!-- BODY -->
    <div class="px-2 py-3">
      <div class="row mb-4 justify-content-end">

        <!-- Cargar XLSX -->
        <button class="btn btn-gradiente-azul px-4 fw-semibold col-md-3"
          @click="premios.length ? abrirConfirmacion() : triggerFile()">
          <i class="pi pi-upload me-2"></i>
          {{ premios.length ? 'Cambiar Base de Datos (.xlsx)' : 'Cargar Base de Datos (.xlsx)' }}
        </button>

        <!-- Crear Premio -->
        <div class="col-md-3 d-flex justify-content-end">
          <button class="btn btn-gradiente-azul w-100 fw-semibold" @click="abrirModalCrear">
            <i class="pi pi-plus me-2"></i> Agregar premio
          </button>
        </div>

        <!-- Filtro -->
        <div class="filtro-mini d-flex align-items-center bg-white px-2 rounded-3 col-md-3">
          <i class="pi pi-search text-primary small"></i>
          <input
            type="text"
            class="filtro-mini-input"
            placeholder="Buscar premio..."
            v-model="filtroNombre"
            @input="buscarPorNombre"
          />
        </div>
      </div>

      <!-- TABLA -->
      <div class="table-responsive table-wrapper">
        <table class="table tabla-compacta align-middle">
          <thead>
            <tr>
              <th>#</th>
              <th>Nombre</th>
              <th>Descripción</th>
              <th>Orden</th>
              <th>Estatus</th>
              <th class="text-center">Acción</th>
            </tr>
          </thead>

          <tbody>
            <!-- Sin datos -->
            <tr v-if="premios.length === 0">
              <td colspan="6" class="text-center text-muted py-3">
                <i class="pi pi-info-circle me-2"></i>
                No hay premios para mostrar.
              </td>
            </tr>

            <!-- Datos -->
            <tr v-for="(p, index) in premios" :key="p.id">
              <td>{{ index + 1 }}</td>
              <td class="fw-semibold">{{ p.nombre }}</td>
              <td class="text-muted" style="max-width: 260px; white-space: normal;">
                {{ p.descripcion || '—' }}
              </td>
              <td>{{ p.orden }}</td>

              <td>
                <span
                  class="estado"
                  :class="{
                    'estado-pendiente': !p.estatusNombre || p.estatusNombre === 'Pendiente',
                    'estado-sorteado': p.estatusNombre === 'Sorteado',
                    'estado-entregado': p.estatusNombre === 'Entregado'
                  }"
                >
                  {{ p.estatusNombre || 'Pendiente' }}
                </span>
              </td>

              <td class="text-center">
                <button class="btn btn-sm me-2" @click="abrirEditor(p)">
                  <i class="pi pi-pencil"></i>
                </button>

                <button class="btn btn-sm" @click="confirmarEliminarUnidad(p.id)">
                  <i class="pi pi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- FOOTER + PAGINADO -->
      <div class="d-flex flex-column align-items-center mt-4">
        <input ref="inputArchivo" type="file" accept=".xlsx,.xls" class="d-none" @change="procesarArchivo" />

        <!-- PAGINADOR -->
        <div class="paginador-container d-flex align-items-center gap-2 mt-3" v-if="true">

          <!-- Botón anterior -->
          <button
            class="pag-btn"
            :disabled="pageNumber === 1"
            @click="cambiarPagina(pageNumber - 1)"
          >
            <i class="pi pi-angle-left"></i>
          </button>

          <!-- Botones numerados -->
          <button
            v-for="n in totalPages"
            :key="n"
            class="pag-btn"
            :class="{ active: n === pageNumber }"
            @click="cambiarPagina(n)"
          >
            {{ n }}
          </button>

          <!-- Botón siguiente -->
          <button
            class="pag-btn"
            :disabled="pageNumber === totalPages"
            @click="cambiarPagina(pageNumber + 1)"
          >
            <i class="pi pi-angle-right"></i>
          </button>

          <!-- Selector de cantidad -->
          <div class="page-size-box ms-3 d-flex align-items-center">
            <label class="me-2 fw-semibold">Mostrar:</label>
            <select
              v-model="pageSize"
              @change="cambiarPageSize"
              class="form-select form-select-sm w-auto"
            >
              <option :value="10">10</option>
              <option :value="20">20</option>
              <option :value="50">50</option>
              <option :value="100">100</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- MODALES (igual que antes) -->
    <div v-if="modalConfirmar" class="modal-confirm">
      <div class="modal-confirm-box">
        <h4 class="fw-bold text-primary mb-2">¿Reemplazar premios?</h4>
        <p class="text-muted small">Esto eliminará todos los premios actuales del sorteo.</p>

        <div class="d-flex gap-3 justify-content-center mt-3">
          <button class="btn btn-secondary" @click="modalConfirmar = false">No</button>
          <button class="btn btn-danger" @click="confirmarEliminacion">Sí, eliminar todo</button>
        </div>
      </div>
    </div>

    <div v-if="modalEliminarUnidad" class="modal-confirm">
      <div class="modal-confirm-box">
        <h4 class="fw-bold text-danger mb-2">¿Eliminar este premio?</h4>

        <div class="d-flex gap-3 justify-content-center mt-3">
          <button class="btn btn-secondary" @click="modalEliminarUnidad = false">Cancelar</button>
          <button class="btn btn-danger" @click="eliminarPremioUnidad">Eliminar</button>
        </div>
      </div>
    </div>

    <div v-if="modalCargaExitosa" class="modal-confirm">
      <div class="modal-confirm-box">
        <h4 class="fw-bold text-success mb-2">¡Carga exitosa!</h4>
        <p class="text-muted small">La base de premios fue cargada correctamente.</p>

        <button class="btn btn-primary mt-3" @click="modalCargaExitosa = false">Entendido</button>
      </div>
    </div>

    <div v-if="modalCrear" class="modal-confirm">
      <div class="modal-confirm-box" style="max-width: 500px;">
        <h4 class="fw-bold text-primary mb-3">Nuevo premio</h4>

        <input v-model="nuevoNombre" type="text" class="form-control mb-3" placeholder="Nombre del premio" />
        <textarea
          v-model="nuevoDescripcion"
          class="form-control mb-3"
          placeholder="Descripción del premio (opcional)"
          style="min-height: 90px;"
        ></textarea>

        <div class="d-flex gap-3 justify-content-center mt-2">
          <button class="btn btn-secondary" @click="modalCrear = false">Cancelar</button>
          <button class="btn btn-primary" @click="crearPremio">Guardar premio</button>
        </div>
      </div>
    </div>

    <div v-if="modalEditar" class="modal-confirm">
      <div class="modal-confirm-box" style="max-width: 500px;">
        <h4 class="fw-bold text-primary mb-3">Editar premio</h4>

        <input v-model="premioEditandoTexto" type="text" class="form-control mb-3" />
        <textarea
          v-model="premioEditandoDescripcion"
          class="form-control mb-3"
          placeholder="Descripción del premio"
          style="min-height: 90px;"
        ></textarea>

        <div class="d-flex gap-3 justify-content-center mt-2">
          <button class="btn btn-secondary" @click="cerrarEditor">Cancelar</button>
          <button class="btn btn-primary" @click="guardarEdicion">Guardar cambios</button>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
import api from "@/utilities/api";
import * as XLSX from "xlsx";
import { useToast } from "primevue/usetoast";

export default {
  name: "PremiosView",
  props: { sorteoId: Number },

  setup() {
    const toast = useToast();
    return { toast };
  },

  data() {
    return {
      premios: [],
      filtroNombre: "",
      premioNuevo: "",
      modalCrear: false,
      nuevoNombre: "",
      nuevoDescripcion: "",
      modalEditar: false,
      premioEditandoId: null,
      premioEditandoTexto: "",
      premioEditandoDescripcion: "",
      premioEditandoOrden: 0,
      modalConfirmar: false,
      modalEliminarUnidad: false,
      idUnidadEliminar: null,
      modalCargaExitosa: false,

      // PAGINACIÓN
      pageNumber: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 1,
    };
  },

  methods: {
    /* ===========================
       CARGA PRINCIPAL DE PREMIOS
       ===========================*/
    async cargarPremios() {
      try {
        const params = {};

        if (this.filtroNombre.trim()) {
          params.nombre = this.filtroNombre.trim();
        }

        const { data } = await api.get(
          `/sorteos/${this.sorteoId}/premios/${this.pageSize}/${this.pageNumber}`,
          { params }
        );

        this.premios = data.rows;
        this.totalCount = data.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);

        // Scroll suave hacia arriba al cambiar de página
        const wrapper = document.querySelector(".table-wrapper");
        if (wrapper) wrapper.scrollTo({ top: 0, behavior: "smooth" });

      } catch (e) {
        console.error("Error cargando premios:", e);
      }
    },

    /* ===========================
       PAGINACIÓN NUEVA
       ===========================*/
    cambiarPagina(nuevaPagina) {
      if (nuevaPagina < 1 || nuevaPagina > this.totalPages) return;

      this.pageNumber = nuevaPagina;
      this.cargarPremios();
    },

    cambiarPageSize() {
      this.pageNumber = 1; // Reinicia a la primera página
      this.cargarPremios();
    },

    buscarPorNombre() {
      this.pageNumber = 1;
      this.cargarPremios();
    },

    /* ===========================
       CRUD DE PREMIOS
       ===========================*/
    abrirModalCrear() {
      this.nuevoNombre = "";
      this.nuevoDescripcion = "";
      this.modalCrear = true;
    },

    async crearPremio() {
      if (!this.nuevoNombre.trim()) return;

      try {
        await api.post(`/sorteos/${this.sorteoId}/premios`, {
          nombre: this.nuevoNombre,
          descripcion: this.nuevoDescripcion,
          orden: this.premios.length + 1,
        });

        this.modalCrear = false;
        await this.cargarPremios();

        this.toast.add({
          severity: "success",
          summary: "Premio agregado",
          life: 3000,
        });

      } catch (e) {
        console.error(e);
      }
    },

    abrirEditor(p) {
      this.premioEditandoId = p.id;
      this.premioEditandoTexto = p.nombre;
      this.premioEditandoDescripcion = p.descripcion || "";
      this.premioEditandoOrden = p.orden || 0;
      this.modalEditar = true;
    },

    cerrarEditor() {
      this.modalEditar = false;
    },

    async guardarEdicion() {
      if (!this.premioEditandoTexto.trim()) return;

      try {
        await api.put(`/sorteos/${this.sorteoId}/premios/${this.premioEditandoId}`, {
          nombre: this.premioEditandoTexto,
          descripcion: this.premioEditandoDescripcion,
          orden: this.premioEditandoOrden,
        });

        this.modalEditar = false;
        await this.cargarPremios();

        this.toast.add({
          severity: "success",
          summary: "Premio actualizado",
          life: 3000,
        });

      } catch (e) {
        console.error(e);
      }
    },

    triggerFile() {
      this.$refs.inputArchivo.click();
    },

    mapearPremio(item) {
      return {
        nombre: item.Nombre || item.nombre || item.Premio || "",
        descripcion: item.Descripcion || item.descripcion || "",
        orden: item.Orden || item.orden || 0,
      };
    },

    async procesarArchivo(e) {
      const file = e.target.files[0];
      if (!file) return;

      try {
        const buffer = await file.arrayBuffer();
        const workbook = XLSX.read(buffer);
        const hoja = workbook.Sheets[workbook.SheetNames[0]];
        const json = XLSX.utils.sheet_to_json(hoja, { defval: "" });

        let lista = json.map(this.mapearPremio).filter((x) => x.nombre.trim() !== "");

        await api.post(`/sorteos/${this.sorteoId}/premios/json`, {
          premios: lista,
        });

        await this.cargarPremios();
        this.modalCargaExitosa = true;

      } catch (err) {
        console.log(err);

        this.toast.add({
          severity: "error",
          summary: "Error al cargar XLSX",
          detail: "Archivo inválido o datos duplicados.",
          life: 4500,
        });
      }
    },

    abrirConfirmacion() {
      this.modalConfirmar = true;
    },

    async confirmarEliminacion() {
      try {
        await api.delete(`/sorteos/${this.sorteoId}/premios`);
        this.premios = [];
        this.modalConfirmar = false;

      } catch (e) {
        console.error(e);
      }
    },

    confirmarEliminarUnidad(id) {
      this.idUnidadEliminar = id;
      this.modalEliminarUnidad = true;
    },

    async eliminarPremioUnidad() {
      try {
        await api.delete(`/sorteos/${this.sorteoId}/premios/${this.idUnidadEliminar}`);

        this.modalEliminarUnidad = false;
        await this.cargarPremios();

      } catch (e) {
        console.error(e);
      }
    },
  },

  mounted() {
    this.cargarPremios();
  },
};
</script>


<style scoped>
/* ====== Reutiliza el estilo institucional ====== */
.header-gradiente {
  /* background: linear-gradient(90deg, #003870, #0058b0); */
  background: linear-gradient(90deg, #0752a1, #0e63e2);
  color: white;
  border-radius: 0.6rem;
  padding: 1rem 1.5rem;
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);
}

.filtro-mini {
  display: flex;
  align-items: center;
  background: white;
  border-radius: 0.6rem;
  padding: 0.6rem 1.3rem;

  border: 1px solid rgba(0, 56, 112, 0.3);
}

.filtro-mini-input {
  border: none;
  outline: none;
  background: transparent;
  font-size: 0.85rem;
  width: 100%;
  margin-left: 0.5rem;
}

/* TABLA */
.table-wrapper {
  position: relative;
  height: 60vh; /* Ajusta a tu gusto */
  display: flex;
  flex-direction: column;
  max-height: 70vh;
  overflow-y: auto;
  background-color: white;
  border-radius: 0.6rem;
  box-shadow: inset 0 0 8px rgba(0, 0, 0, 0.05);
}

.table-wrapper table {
  flex: 1;
}


.tabla-compacta th {
  position: sticky;
  top: 0;
  background: #0856a5;
  /* background: linear-gradient(90deg, #007bff, #0056d6); */
  color: white;
  padding: 0.8rem;
  text-transform: uppercase;
  font-size: 0.85rem;
  z-index: 10;
}

.tabla-compacta td {
  padding: 0.75rem;
}

.tabla-compacta tbody tr:hover {
  background-color: #f1f5fb;
}

/* ESTADOS */
.estado {
  padding: 0.35em 0.7em;
  border-radius: 0.5rem;
  font-weight: 600;
  font-size: 0.8rem;
}

.estado-pendiente {
  background: #9ca3af;
  color: white;
}

.estado-sorteado {
  background: #3b82f6;
  color: white;
}

.estado-entregado {
  background: #22c55e;
  color: white;
}

/* BOTÓN PRINCIPAL */
.btn-gradiente-azul {
  background: linear-gradient(90deg, #007bff, #0056d6);
  color: white;
  border: none;
  border-radius: 0.6rem;
  padding: 0.6rem 1.3rem;
  transition: all 0.3s ease;
}

.btn-gradiente-azul:hover {
  transform: scale(1.04);
  box-shadow: 0 0 12px rgba(0, 86, 214, 0.4);
}

/* MODALES */
.modal-confirm {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3000;
}

.modal-confirm-box {
  background: white;
  padding: 1.8rem;
  border-radius: 0.8rem;
  width: 90%;
  max-width: 420px;
  text-align: center;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
  animation: scaleIn 0.3s ease;
}

/* SCROLLBAR */
.table-wrapper::-webkit-scrollbar {
  width: 8px;
}

.table-wrapper::-webkit-scrollbar-thumb {
  background: rgba(0, 56, 112, 0.4);
  border-radius: 10px;
}

.table-wrapper::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 56, 112, 0.6);
}

/* ===========================
   PAGINADOR ESTILO INSTITUCIONAL
   ===========================*/
.paginador-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.45rem;
  margin-top: 1.5rem;
  flex-wrap: wrap;
}

.pag-btn {
  border: 1px solid #0752a1;
  background: white;
  color: #0752a1;
  padding: 6px 12px;
  border-radius: 0.45rem;
  cursor: pointer;
  font-size: 0.85rem;
  transition: all 0.25s ease;
  min-width: 40px;
  text-align: center;
}

.pag-btn:hover {
  background: #0752a1;
  color: white;
  transform: translateY(-1px);
}

.pag-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  transform: none;
}

.pag-btn.active {
  background: #0752a1;
  color: white;
  border-color: #0752a1;
  font-weight: 600;
}

/* Selector del tamaño de página */
.page-size-box {
  display: flex;
  align-items: center;
  gap: 6px;
}

.page-size-box select {
  border: 1px solid #0752a1 !important;
  border-radius: 0.45rem;
  padding: 4px 8px;
  font-size: 0.85rem;
  cursor: pointer;
}

.page-size-box select:focus {
  outline: none;
  box-shadow: 0 0 0 2px rgba(7, 82, 161, 0.25);
}


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
</style>
