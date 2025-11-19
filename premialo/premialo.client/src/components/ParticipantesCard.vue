<template>
  <div class="admin-card animate__animated animate__fadeInUp">
    <h5 class="card-title">
      <i class="bi bi-people-fill me-2 text-info"></i> Gestión de Participantes
    </h5>
    <p class="small text-light opacity-75 mb-3">
      Ver la lista completa de participantes.
    </p>

    <!--Botón principal -->
    <button class="btn btn-primary w-100 fw-bold" @click="$emit('ver-participantes')">
      <i class="pi pi-users fs-5 me-1"></i> Ver Participantes
    </button>



      <!-- SECCIÓN DE TOTALES -->
    <div class="totales-container mt-4">
      <div class="total-item">
        <label> Total Participantes:</label>
        <span class="valor">{{ totalParticipantes }}</span>
      </div>

      <div class="total-item">
        <label>Participantes Concursandos:</label>
        <span class="valor">{{ totalConcursando }}</span>
      </div>
    </div>



  </div>
</template>

<script>
export default {
  name: "ParticipantesCard",
  props: {
    totalParticipantes: { type: Number, default: 0 },
    totalConcursando: { type: Number, default: 0 },
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
