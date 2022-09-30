<template>
  <ul :class="{ 'menu-list': !isSubmenuList }">
    <aside-menu-item
      v-for="(item, index) in menu"
      :key="index"
      :item="item"
      :isDesktopScreenAndBelow="isDesktopScreenAndBelow"
      @menu-click="menuClick"
    />
  </ul>
</template>

<script>
import AsideMenuItem from '@/components/AsideMenuItem.vue'

export default {
  name: 'AsideMenuList',
  components: {
    AsideMenuItem
  },
  props: {
    isSubmenuList: Boolean,
    isDesktopScreenAndBelow:{
      type: Boolean,
      default: () => false
    },
    menu: {
      type: Array,
      default: () => []
    }
  },
  emits: ['menu-click'],
  methods: {
    menuClick (item) {
      this.$emit('menu-click', item)
    }
  }
}
</script>
