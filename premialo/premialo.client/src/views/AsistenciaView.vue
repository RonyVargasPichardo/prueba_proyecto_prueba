<template>
  <div class="asistencia-view container py-4 animate__animated animate__fadeIn">
    <h3 class="fw-bold text-info mb-4">
      <i class="bi bi-person-check-fill me-2"></i> Registro de Asistencia
    </h3>

    <!-- 🔹 Totales -->
    <div class="totales-box mb-4">
      <p><strong>Total Asistencias:</strong> {{ totales.asistencias }}</p>
      <p><strong>Concursando:</strong> {{ totales.concursando }}</p>
    </div>

    <!-- 🔹 Input de búsqueda -->
    <div class="input-busqueda mt-3">
      <label class="form-label fw-bold">Buscar cédula</label>
      <div class="d-flex gap-2">
        <input
          v-model="cedula"
          class="form-control"
          placeholder="Ingrese cédula"
          @keyup.enter="buscarCedula"
        />
        <button class="btn btn-primary fw-semibold" @click="buscarCedula">
          <i class="bi bi-search me-1"></i> Buscar
        </button>
      </div>
    </div>

    <!-- 🔹 Datos del participante -->
    <div v-if="datos" class="datos-box mt-4">
      <p><strong>Cédula:</strong> {{ datos.cedula }}</p>
      <p><strong>Nombre:</strong> {{ datos.nombres }} {{ datos.apellidos }}</p>
      <p><strong>Departamento:</strong> {{ datos.departamento || "—" }}</p>
    </div>

    <!-- 🔹 Botón Registrar -->
    <div v-if="datos" class="text-center mt-4">
      <button class="btn btn-success btn-lg fw-bold" @click="registrar">
        <i class="bi bi-check-circle me-2"></i> Registrar Asistencia
      </button>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import api from "@/utilities/api";
import { useToast } from "primevue/usetoast";

export default {
  name: "AsistenciaView",
  props: {
    sorteoId: Number,
  },
  setup(props) {
    const toast = useToast();
    const cedula = ref("");
    const datos = ref(null);
    const totales = ref({
      asistencias: 0,
      concursando: 0,
    });

    // Cargar totales
    const cargarTotales = async () => {
      try {
        const r = await api.get(`/sorteos/${props.sorteoId}/asistencia`);
        totales.value.asistencias = r.data.asistenciasTotal;
        totales.value.concursando = r.data.concursando;
      } catch (err) {
        console.error(err);
      }
    };

    // Buscar cédula
    const buscarCedula = async () => {
      if (!cedula.value.trim()) {
        toast.add({
          severity: "warn",
          summary: "Atención",
          detail: "Ingrese una cédula válida.",
          life: 3000,
        });
        return;
      }

      try {
        const r = await api.get(`/sorteos/${cedula.value}/asistencia/cedulas`);
        datos.value = r.data;

        toast.add({
          severity: "success",
          summary: "Datos encontrados",
          detail: `${datos.value.nombres} ${datos.value.apellidos}`,
          life: 2500,
        });
      } catch (e) {
        datos.value = null;
        toast.add({
          severity: "error",
          summary: "No encontrado",
          detail: "La cédula no existe en RRHH.",
          life: 3000,
        });
      }
    };

    // Registrar asistencia
    const registrar = async () => {
      try {
        await api.post(`/sorteos/${props.sorteoId}/asistencia`, {
          cedula: datos.value.cedula,
        });

        toast.add({
          severity: "success",
          summary: "Asistencia registrada",
          detail: "Participante registrado correctamente.",
          life: 3000,
        });

        datos.value = null;
        cedula.value = "";
        cargarTotales();
      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "No se pudo registrar la asistencia.",
          life: 3000,
        });
      }
    };

    onMounted(() => {
      cargarTotales();
    });

    return {
      cedula,
      datos,
      totales,
      buscarCedula,
      registrar,
    };
  },
};
</script>

<style scoped>
.asistencia-view {
  color: #fff;
}

.totales-box {
  background: rgba(255, 255, 255, 0.08);
  padding: 1rem;
  border-radius: 0.6rem;
  text-align: center;
  font-size: 1.1rem;
  border: 1px solid rgba(255, 255, 255, 0.15);
}

.datos-box {
  background: rgba(255, 255, 255, 0.08);
  padding: 1rem;
  border-radius: 0.6rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.form-control {
  background-color: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 0.5rem;
}

button.btn-primary {
  background: linear-gradient(135deg, #0058b0, #007bff);
  border: none;
}

button.btn-primary:hover {
  background: linear-gradient(135deg, #007bff, #009dff);
}
</style>
