<template>
    <div class="admin-layout container-fluid py-2 animate__animated animate__fadeIn">
        
        <!-- 🔹 HEADER (reutilizado para todos los roles) -->
        <Header
            :perfilUsuario="perfilUsuario"
            :rol="rolUsuario"
            @cerrar-sesion="cerrarSesion"
        />

        <!-- 🔹 CONTENEDOR PRINCIPAL -->
        <div class="layout-body">
            
            <!-- SIDEBAR EXCLUSIVO DE ADMIN -->
            <SidebarAdmin />

            <!-- ÁREA DE CONTENIDO -->
            <main class="contenido-principal">
                <transition name="fade-slide" mode="out-in">
                    <router-view />
                </transition>
            </main>

        </div>

    </div>
</template>

<script>
import Header from "@/components/Header.vue";
import SidebarAdmin from "@/components/admin/SidebarAdmin.vue";

export default {
    name: "AdminLayoutView",

    components: {
        Header,
        SidebarAdmin,
    },

    data() {
        return {
            perfilUsuario: localStorage.getItem("nombre") || "Administrador",
            rolUsuario: localStorage.getItem("rol") || "Admin",
        };
    },

    methods: {
        cerrarSesion() {
            localStorage.removeItem("nombre");
            localStorage.removeItem("rol");
            localStorage.removeItem("token");
            this.$router.push("/login");
        },
    },
};
</script>

<style scoped>
/* ============================
   ESTRUCTURA GENERAL
============================ */
.admin-layout {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
    color: #fff;
}

/* ============================
   CONTENEDOR PRINCIPAL
============================ */
.layout-body {
    display: flex;
    flex: 1;
    margin-top: 1rem;
    gap: 1rem;
}

/* ============================
   CONTENIDO PRINCIPAL
============================ */
.contenido-principal {
    flex: 1;
    padding: 2rem;
    background: rgba(255, 255, 255, 0.06);
    border-radius: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.1);
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.25);
    backdrop-filter: blur(8px);
    overflow-y: auto;
}

/* ============================
   TRANSICIONES
============================ */
.fade-slide-enter-active,
.fade-slide-leave-active {
    transition: all 0.35s ease;
}

.fade-slide-enter-from {
    opacity: 0;
    transform: translateY(15px);
}

.fade-slide-leave-to {
    opacity: 0;
    transform: translateY(-15px);
}
</style>
