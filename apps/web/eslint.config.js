import pluginVue from 'eslint-plugin-vue'
import vueTsConfigs from '@vue/eslint-config-typescript'
import prettier from '@vue/eslint-config-prettier'

export default [
  { ignores: ['dist/**', 'node_modules/**'] },
  ...pluginVue.configs['flat/recommended'],
  ...vueTsConfigs(),
  prettier,
  {
    rules: {
      'vue/multi-word-component-names': 'off'
    }
  }
]
