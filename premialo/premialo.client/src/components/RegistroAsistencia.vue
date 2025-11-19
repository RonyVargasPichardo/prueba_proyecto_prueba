<template>
  <div class="admin-card animate__animated animate__fadeInUp">
    <h5 class="card-title">
      <i class="bi bi-person-check-fill me-2 text-info"></i>
      Registro de Asistencia
    </h5>
     <p class="small text-light opacity-75 mb-3">
      Ver la lista completa de Asistencia.
    </p>

    <!-- BOTÓN REGISTRAR -->
    <div class="text-center mt-3">
      <button
       class="btn btn-primary w-100 fw-bold"
        @click="$emit('registrar-asistencia')"
      >
        Registrar
      </button>
    </div>

    <!-- SECCIÓN DE TOTALES -->
    <div class="totales-container mt-4">
      <div class="total-item">
        <label>Total Asistencia:</label>
        <span class="valor">{{ totalAsistencia }}</span>
      </div>

      <div class="total-item">
        <label>Total Concursando:</label>
        <span class="valor">{{ totalConcursando }}</span>
      </div>
    </div>

  </div>
</template>

<script>
import { useToast } from "primevue/usetoast";
import { ref } from "vue";

export default {
  name: "RegistroAsistencia",
   props: {
    totalAsistencia: { type: Number, default: 0 },
    totalConcursando: { type: Number, default: 0 }
  },
  setup(props, { emit }) {
    const cedula = ref("");
    const toast = useToast();

    const registrarAsistencia = () => {
      if (!cedula.value.trim()) {
        toast.add({
          severity: "warn",
          summary: "Atención",
          detail: "Ingrese una cédula válida.",
          life: 3000,
        });
        return;
      }

      // 🔹 Normaliza la cédula (quita guiones o espacios)
      const cedulaBuscar = cedula.value.replace(/[-\s]/g, "").trim();

      // 🔹 Buscar participante
      const encontrado = props.participantes.find(
        (p) => p.cedula === cedulaBuscar
      );

      if (!encontrado) {
        toast.add({
          severity: "error",
          summary: "No encontrado",
          detail: "Cédula no registrada.",
          life: 3000,
        });
        cedula.value = "";
        return;
      }

      // 🔹 Verificar si ya estaba activo
      if (encontrado.activo) {
        toast.add({
          severity: "info",
          summary: "Ya registrada",
          detail: `${encontrado.nombre} ya tiene asistencia registrada.`,
          life: 3000,
        });
        cedula.value = "";
        return;
      }

      // 🔹 Simular actualización (PUT)
      encontrado.activo = true;

      toast.add({
        severity: "success",
        summary: "Asistencia registrada",
        detail: `Se registró a ${encontrado.nombre}.`,
        life: 3000,
      });

      // 🔹 Emitir evento al padre para actualizar totales
      emit("actualizar-participantes");

      // 🔹 Limpiar input
      cedula.value = "";
    };

    return { cedula, registrarAsistencia };
  },
};
</script>

<style scoped>
.admin-card {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(8px);
  border-radius: 1rem;
  padding: 1.5rem;
  box-shadow: 0 3px 12px rgba(0, 0, 0, 0.2);
  transition: all 0.3s ease;
  height: 100%;
}

.admin-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 4px 15px rgba(255, 255, 255, 0.1);
}

.card-title {
  font-weight: 700;
  color: #5cc4ff;
}

/* Contenedor de totales */
.totales-container {
  background: rgba(255, 255, 255, 0.08);
  border-radius: 0.7rem;
  padding: 1rem 1.2rem;
  border: 1px solid rgba(255, 255, 255, 0.12);
}

.total-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.4rem;
  font-size: 1rem;
}

.total-item:last-child {
  margin-bottom: 0;
}

.total-item label {
  color: #dbeafe;
  font-weight: 600;
}

.valor {
  color: #fff;
  font-weight: 700;
}
</style>

