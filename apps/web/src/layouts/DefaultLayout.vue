<script setup lang="ts"></script>
<template>
  <div class="min-h-screen bg-background text-foreground">
    <header class="border-b border-border">
      <div class="container flex h-14 items-center justify-between">
        <span class="font-medium">FPT Connect</span>
        <nav class="text-sm text-muted-foreground">Default layout</nav>
      </div>
    </header>
    <main class="container py-6"><slot /></main>
  </div>
</template>
