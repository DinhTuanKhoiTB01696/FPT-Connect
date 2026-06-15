<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import AuthLayout from '@/layouts/AuthLayout.vue'
import BlankLayout from '@/layouts/BlankLayout.vue'

const route = useRoute()
const layouts = { default: DefaultLayout, auth: AuthLayout, blank: BlankLayout }
const layout = computed(() => layouts[(route.meta.layout as keyof typeof layouts) ?? 'default'] ?? DefaultLayout)
</script>

<template>
  <component :is="layout">
    <RouterView />
  </component>
</template>
