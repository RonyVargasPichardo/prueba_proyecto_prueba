<template>
    <aside class="sidebar-menu animate__animated animate__fadeInLeft">
        <nav class="nav flex-column">
            <button
                v-for="item in menuItems"
                :key="item.route"
                class="nav-link"
                :class="{ active: isActive(item.route) }"
                @click="navegar(item.route)"
            >
                <i :class="item.icon" class="me-2"></i>
                {{ item.label }}
            </button>
        </nav>
    </aside>
</template>

<script>
export default {
    name: "SidebarMenu",

    computed: {
        currentRoute() {
            return this.$route.path;
        },
    },

    methods: {
        navegar(ruta) {
            const id = this.$route.params.id;
            if (!id) return; // protección
            this.$router.push(`/sorteos/${id}/${ruta}`);
        },

        isActive(ruta) {
            // A prueba de errores
            return this.$route.path.endsWith(`/${ruta}`);
        },
    },

    data() {
        return {
            menuItems: [
                { label: "Configuración", route: "configuracion", icon: "bi bi-gear-fill" },
                { label: "Asistencia", route: "asistencia", icon: "bi bi-person-check-fill" },
                { label: "Participantes", route: "participantes", icon: "bi bi-people-fill" },
                { label: "Premios", route: "premios", icon: "bi bi-gift-fill" },
                { label: "Resultados", route: "resultados", icon: "bi bi-graph-up-arrow" },
            ],
        };
    },
};
</script>

<style scoped>
.sidebar-menu {
    position: sticky;
    top: 0;
    z-index: 90;

    flex: 0 0 260px;
    height: 100vh;

    background: rgba(255, 255, 255, 0.08);
    backdrop-filter: blur(10px);

    border-right: 1px solid rgba(255, 255, 255, 0.15);
    border-radius: 0.8rem;

    padding: 1rem 0.5rem;
}

.nav-link {
    color: #e0e6f0;
    background: transparent;
    border: none;
    text-align: left;
    padding: 0.7rem 1rem;
    border-radius: 0.6rem;
    transition: all 0.25s ease;
    font-weight: 500;
}

.nav-link:hover {
    background: rgba(255, 255, 255, 0.12);
    color: #5cc4ff;
}

.nav-link.active {
    background: linear-gradient(135deg, #004b99, #007bff);
    color: white;
    box-shadow: 0 0 10px rgba(0, 123, 255, 0.3);
}
</style>
