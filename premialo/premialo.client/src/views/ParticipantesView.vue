<template>
  <div class="participantes-view animate__animated animate__fadeIn">
    <!-- HEADER -->
    <div class="header-gradiente d-flex align-items-center justify-content-between mb-3">
      <h4 class="fw-bold text-white mb-0">
        <i class="pi pi-users me-2"></i> Lista de Participantes
      </h4>

      <div class="filtro-mini d-flex align-items-center bg-white px-2 rounded-3">
        <i class="pi pi-search text-primary small"></i>
        <input
          type="text"
          v-model="filtroCedula"
          class="filtro-mini-input"
          placeholder="Buscar cédula..."
        />
      </div>
    </div>

    <!-- BODY -->
    <div class="table-responsive table-wrapper">
      <table class="table tabla-compacta align-middle">
        <thead>
          <tr>
            <th>#</th>
            <th>Nombre</th>
            <th>Cédula</th>
            <th>Departamento</th>
            <th>Cargo</th>
            <th class="text-center">Estatus</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(p, index) in participantesFiltrados"
            :key="index"
            :class="{ 'fila-activa': p.estatus === 'Concursando' }"
          >
            <td>{{ index + 1 }}</td>
            <td class="fw-semibold">{{ p.nombres }} {{ p.apellidos }}</td>
            <td>{{ p.documentoIdentidad }}</td>
            <td>{{ p.unidadOrganizativa }}</td>
            <td>{{ p.cargo }}</td>
            <td class="text-center">
              <span class="estado" :class="getClaseEstatus(p.estatus)">
                {{ p.estatus }}
              </span>
            </td>
          </tr>

          <tr v-if="participantes.length === 0">
            <td colspan="6" class="text-center text-muted py-3">
              <i class="pi pi-users me-2"></i>
              Aún no se han cargado participantes para este sorteo.
            </td>
          </tr>

          <tr v-else-if="participantesFiltrados.length === 0">
            <td colspan="6" class="text-center text-muted py-3">
              <i class="pi pi-info-circle me-2"></i>
              No se encontraron participantes con esa cédula.
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- FOOTER -->
    <div class="d-flex justify-content-center gap-3 mt-4">
      <button class="btn btn-gradiente-azul px-4 fw-semibold"
        @click="hayParticipantes ? abrirConfirmacion() : triggerFile()">
        <i class="pi pi-upload me-2"></i>
        {{ hayParticipantes ? 'Cambiar Base de Datos (.xlsx)' : 'Cargar Base de Datos (.xlsx)' }}
      </button>

      <input ref="inputArchivo" type="file" accept=".xlsx,.xls" class="d-none" @change="procesarArchivo" />
    </div>

    <!-- MODALES -->
    <div v-if="modalConfirmar" class="modal-confirm">
      <div class="modal-confirm-box">
        <h4 class="fw-bold text-primary mb-2">¿Reemplazar participantes?</h4>
        <p class="text-muted small">Esto eliminará todos los participantes actuales del sorteo.</p>
        <div class="d-flex gap-3 justify-content-center mt-3">
          <button class="btn btn-secondary" @click="modalConfirmar = false">No</button>
          <button class="btn btn-danger" @click="confirmarEliminacion">Sí, reemplazar</button>
        </div>
      </div>
    </div>

    <div v-if="modalResultado" class="modal-confirm">
      <div class="modal-confirm-box">
        <h4 class="fw-bold text-success mb-2">Participantes eliminados</h4>
        <p class="text-muted small">Ahora puedes subir una nueva base de datos.</p>
        <div class="d-flex gap-3 justify-content-center mt-3">
          <button class="btn btn-primary" @click="subirNuevaBase">Subir nueva Base (.xlsx)</button>
          <button class="btn btn-secondary" @click="cerrarResultado">Cerrar</button>
        </div>
      </div>
    </div>

    <div v-if="modalCargaExitosa" class="modal-confirm">
      <div class="modal-confirm-box">
        <h4 class="fw-bold text-success mb-2">¡Carga exitosa!</h4>
        <p class="text-muted small">La base de datos se cargó correctamente.</p>
        <div class="d-flex gap-3 justify-content-center mt-3">
          <button class="btn btn-primary" @click="modalCargaExitosa = false">Entendido</button>
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
  name: "ParticipantesView",
  props: {
    sorteoId: { type: Number, required: true },
  },
  setup() {
    const toast = useToast();
    return { toast };
  },
  data() {
    return {
      filtroCedula: "",
      participantes: [],
      modalCargaExitosa: false,
      modalConfirmar: false,
      modalResultado: false,
    };
  },
  computed: {
    participantesFiltrados() {
      if (!this.filtroCedula.trim()) return this.participantes;
      const texto = this.filtroCedula.toLowerCase();
      return this.participantes.filter((p) =>
        p.documentoIdentidad?.toLowerCase().includes(texto)
      );
    },
    hayParticipantes() {
      return this.participantes && this.participantes.length > 0;
    },
  },
  methods: {
    async cargarParticipantes() {
      try {
        const { data } = await api.get(`/sorteos/${this.sorteoId}/participantes`);
        this.participantes = data;
      } catch (error) {
        console.error("Error al cargar participantes:", error);
      }
    },
    getClaseEstatus(estatus) {
      switch (estatus) {
        case "Registrado": return "estado-registrado";
        case "Concursando": return "estado-concursando";
        case "Ganador": return "estado-ganador";
        default: return "estado-registrado";
      }
    },
    triggerFile() { this.$refs.inputArchivo.click(); },
    mapearItemExcel(item) {
      return {
        documentoIdentidad: item.documentoIdentidad || item.Cédula || item.Documento || "",
        nombres: item.nombres || item.Nombre || item.Nombres || "",
        apellidos: item.apellidos || item.Apellido || item.Apellidos || "",
        email: item.email || item.Email || "",
        telefono: item.telefono || item.Telefono || "",
        cargo: item.cargo || item.Cargo || "",
        unidadOrganizativa: item.unidadOrganizativa || item.Departamento || item.Unidad || "",
      };
    },
    async procesarArchivo(event) {
      const archivo = event.target.files[0];
      if (!archivo) return;

      try {
        const data = await archivo.arrayBuffer();
        const workbook = XLSX.read(data);
        const hoja = workbook.Sheets[workbook.SheetNames[0]];
        const json = XLSX.utils.sheet_to_json(hoja, { defval: "" });
        let listaMapeada = json.map(this.mapearItemExcel);
        listaMapeada = listaMapeada.filter(
          (x) => x.documentoIdentidad && x.documentoIdentidad.toString().trim().length >= 8
        );
        if (listaMapeada.length === 0) {
          this.toast.add({
            severity: "warn",
            summary: "Archivo inválido",
            detail: "El XLSX no contiene datos válidos.",
            life: 4500,
          });
          return;
        }

        await api.post(`/sorteos/${this.sorteoId}/participantes/bulk`, listaMapeada);
        await this.cargarParticipantes();
        this.toast.add({
          severity: "success",
          summary: "Participantes cargados",
          detail: "La lista se cargó correctamente.",
          life: 4000,
        });
        this.modalCargaExitosa = true;
      } catch (error) {
        console.error("Error procesando archivo:", error);
        this.toast.add({
          severity: "error",
          summary: "Error al procesar archivo",
          detail: "El XLSX contiene datos inválidos o duplicados.",
          life: 4500,
        });
      }
    },
    abrirConfirmacion() { this.modalConfirmar = true; },
    async confirmarEliminacion() {
      try {
        await api.delete(`/sorteos/${this.sorteoId}/participantes`);
        this.participantes = [];
        this.modalConfirmar = false;
        this.modalResultado = true;
      } catch (err) {
        console.error("Error eliminando participantes:", err);
        alert("Error eliminando participantes.");
      }
    },
    subirNuevaBase() {
      this.modalResultado = false;
      this.triggerFile();
    },
    cerrarResultado() {
      this.modalResultado = false;
    },
  },
  mounted() {
    this.cargarParticipantes();
  },
};
</script>

<style scoped>
/* ============================
   HEADER Y FILTRO
============================ */
.header-gradiente {
  background: linear-gradient(90deg, #003870, #0058b0);
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
  padding: 0 0.6rem;
  height: 36px;
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

/* ============================
   TABLA
============================ */
.table-wrapper {
  max-height: 70vh;
  min-height: 300px;
  overflow-y: auto;
  background-color: white;
  border-radius: 0.6rem;
  box-shadow: inset 0 0 8px rgba(0, 0, 0, 0.05);
}
.tabla-compacta th {
  background: #003870;
  color: white;
  padding: 0.8rem;
  text-transform: uppercase;
  font-size: 0.85rem;
}
.tabla-compacta td {
  padding: 0.75rem;
}
.tabla-compacta tbody tr:hover {
  background-color: #f1f5fb;
}
.fila-activa {
  background-color: rgba(22, 163, 74, 0.1);
}

/* ============================
   ESTADOS
============================ */
.estado {
  display: inline-block;
  padding: 0.35em 0.7em;
  border-radius: 0.5rem;
  font-weight: 600;
  font-size: 0.8rem;
}
.estado-registrado { background: #9ca3af; color: white; }
.estado-concursando { background: #3b82f6; color: white; }
.estado-ganador { background: #22c55e; color: white; }

/* ============================
   BOTONES
============================ */
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

/* ============================
   MODALES
============================ */
.modal-confirm {
  position: fixed;
  top: 0; left: 0;
  width: 100%; height: 100%;
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

/* ============================
   SCROLLBAR
============================ */
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

@keyframes scaleIn {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}
</style>
