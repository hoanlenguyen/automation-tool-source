<template>
  <li :class="{ 'is-active': isDropdownActive }">
    <component
      :is="componentIs"
      :to="item.to"
      :href="item.href"
      :target="item.target"
      exact-active-class="is-active"
      :class="{ 'has-icon': !!item.icon, 'has-dropdown-icon': hasDropdown }"
      @click="menuClick"
    >
      <b-icon
        v-if="item.icon"
        :icon="item.icon"
        :class="isDesktopScreenAndBelow?'isFontSize24px':''"
        :title="item.label"
        />      
      <span
        v-if="item.label && !isDesktopScreenAndBelow"
        :class="{ 'menu-item-label': !!item.icon }"
      >
        {{ item.label }} 
      </span>
      <div
        v-if="hasDropdown"
        class="dropdown-icon"
      >
        <b-icon
          :icon="dropdownIcon"
          :class="isDesktopScreenAndBelow?'isFontSize24px':''"
        />
      </div>
    </component>
    <aside-menu-list
      :class="isDesktopScreenAndBelow? '':'pl-3'"
      v-if="hasDropdown"
      :menu="item.menu"
      is-submenu-list
      :isDesktopScreenAndBelow="isDesktopScreenAndBelow"
    />
  </li>
</template>

<script>
export default {
  name: 'AsideMenuItem',
  components: {
    AsideMenuList: () => import('@/components/AsideMenuList.vue')
  },
  props: {
    item: {
      type: Object,
      required: true
    },
    isDesktopScreenAndBelow:{
      type: Boolean,
      default: () => false
    }
  },
  emits: ['menu-click'],
  data () {
    return {
      isDropdownActive: false
    }
  }, 
  computed: {
    componentIs () {
      return this.item.to ? 'router-link' : 'a'
    },
    hasDropdown () {
      return !!this.item.menu
    },
    dropdownIcon () {
      return this.isDropdownActive ? 'minus' : 'plus'
    }
  },
  methods: {
    menuClick () {
      this.$emit('menu-click', this.item)

      if (this.hasDropdown) {
        this.isDropdownActive = !this.isDropdownActive
      }
    } 
  }
}
</script>
