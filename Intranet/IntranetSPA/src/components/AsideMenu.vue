<template>
  <aside
    v-show="isAsideVisible"
    class="aside is-placed-left"
    :style="isDesktopScreenAndBelow?'width: 4rem !important;': ''"
  >
    <div class="aside-tools">
      <a
        class="navbar-item is-hidden-touch is-hidden-widescreen is-desktop-icon-only"
        @click="asideToggleDesktopOnly"
      >
        <b-icon icon="menu" />
      </a>
      <div class="aside-tools-label" v-if="!isDesktopScreenAndBelow">
        <span><b>Intranet</b></span>
      </div>
    </div>
    <div class="menu is-menu-main">
      <template v-for="(menuGroup, index) in menu">
        <p
          v-if="typeof menuGroup === 'string'"
          :key="index"
          class="menu-label"
        >
          {{ menuGroup }}
        </p>
        <aside-menu-list
          v-else
          :key="index "
          :menu="menuGroup"
          :isDesktopScreenAndBelow="isDesktopScreenAndBelow"
          @menu-click="menuClick"
        />
      </template>
    </div>
  </aside>
</template>

<script>
import { mapState } from 'vuex'
import AsideMenuList from '@/components/AsideMenuList.vue'

export default {
  name: 'AsideMenu',
  components: { AsideMenuList },
  props: {
    menu: {
      type: Array,
      default: () => []
    }
  },
  data () {
    return {
      isDesktopScreenAndBelow:false
    }
  },
  created() {
      window.addEventListener('resize', this.handleResize);
      this.handleResize();
    },
  destroyed() {
      window.removeEventListener('resize', this.handleResize);
  },
  computed: {
    ...mapState([
      'isAsideVisible'
    ])
  },
  mounted () {
    this.$router.afterEach(() => {
      this.$store.dispatch('asideDesktopOnlyToggle', false)
    })
  },
  methods: {
    asideToggleDesktopOnly () {
      this.$store.dispatch('asideDesktopOnlyToggle')
    },
    menuClick (item) {
      //
    },
    handleResize(){
      this.isDesktopScreenAndBelow= window.innerWidth <= 1240;
    }
  }
}
</script>
