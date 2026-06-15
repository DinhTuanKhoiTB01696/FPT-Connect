import { defineStore } from 'pinia'
import { ref } from 'vue'

/** Store khung (theme). Chưa chứa nghiệp vụ. */
export const useAppStore = defineStore('app', () => {
  const isDark = ref(false)
  function toggleDark() {
    isDark.value = !isDark.value
    document.documentElement.classList.toggle('dark', isDark.value)
  }
  return { isDark, toggleDark }
})
