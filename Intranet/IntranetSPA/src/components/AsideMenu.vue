<template>
  <section>
    <b-sidebar :fullheight="true" :can-cancel="false" v-model="open">
        <h3 class="is-text-primary has-text-weight-semibold px-4" :style="'background-color:#17191e; color: #fff; line-height:3.5rem'">Intranet</h3>
        <b-menu>
          <b-menu-list>
            <b-menu-item v-for="(item,index) in menu" 
              :icon="item.icon" 
              :label="!item.menu?item.label:''" 
              :to="item.to"
              :tag="(item.menu&&item.menu.length>0)?'a' :'router-link'" 
              :key="index"
              :active="currentRoutePath==item.to"
              >
              <template #label="props">
                  {{item.label}}
                  <b-icon class="is-pulled-right" :icon="props.expanded ? 'menu-down' : 'menu-up'"></b-icon>
              </template>
              <template v-if="item.menu && item.menu.length>0">
                <b-menu-item v-for="(childItem,childIndex) in item.menu" 
                :icon="childItem.icon" 
                :label="childItem.label"
                :to="childItem.to"
                :active="currentRoutePath==childItem.to"
                :key="`child${childIndex}`" 
                :tag="'router-link'">
                </b-menu-item>
              </template>
            </b-menu-item> 
          </b-menu-list>

          <!-- <b-menu-list>
            <b-menu-item label="Logout"></b-menu-item>
          </b-menu-list> -->
        </b-menu>
    </b-sidebar>
  </section>
  <!-- <aside
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
  </aside> -->
</template>

<script>
import { mapState } from 'vuex'
// import AsideMenuList from '@/components/AsideMenuList.vue'

export default {
  name: 'AsideMenu',
  // components: { AsideMenuList },
  props: {
    menu: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      isDesktopScreenAndBelow: false,
      open: true,
      overlay: true,
      fullheight: true,
      fullwidth: false,
      right: false
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
    currentRoutePath() {
        return this.$route.path;
    },
    ...mapState([
      'isAsideVisible'
    ])
  },
  mounted() {
    this.$router.afterEach(() => {
      this.$store.dispatch('asideDesktopOnlyToggle', false)
    })
  },
  methods: {
    asideToggleDesktopOnly() {
      this.$store.dispatch('asideDesktopOnlyToggle')
    },
    menuClick(item) {
      //
    },
    handleResize() {
      this.isDesktopScreenAndBelow = window.innerWidth <= 1240;
    }
  }
}
</script>
<style lang="scss">
// .sidebar-page {
//   display: flex;
//   flex-direction: column;
//   width: 100%;
//   min-height: 100%;

//   // min-height: 100vh;
//   .sidebar-layout {
//     display: flex;
//     flex-direction: row;
//     min-height: 100%;
//     // min-height: 100vh;
//   }
// }
.menu-list>li>a{
  padding-left: 12px;
}
li>a.is-expanded.icon-text{
  padding-left: 12px;
}
a.is-active.is-expanded.icon-text{
  background-color:hsl(0deg, 0%, 96%) !important;
  color: #2e323a;
}
.b-sidebar .sidebar-content{
  width: 14rem !important;
  box-shadow: none;
  background: #2e323a;
}
// @media screen and (max-width: 1023px) {
//   .b-sidebar {
//     .sidebar-content {
//       &.is-mini-mobile {

//         &:not(.is-mini-expand),
//         &.is-mini-expand:not(:hover):not(.is-mini-delayed) {
//           .menu-list {
//             li {
//               a {
//                 span:nth-child(2) {
//                   display: none;
//                 }
//               }

//               ul {
//                 padding-left: 0;

//                 li {
//                   a {
//                     display: inline-block;
//                   }
//                 }
//               }
//             }
//           }

//           .menu-label:not(:last-child) {
//             margin-bottom: 0;
//           }
//         }
//       }
//     }
//   }
// }

// @media screen and (min-width: 1024px) {
//   .b-sidebar {
//     .sidebar-content {
//       &.is-mini {

//         &:not(.is-mini-expand),
//         &.is-mini-expand:not(:hover):not(.is-mini-delayed) {
//           .menu-list {
//             li {
//               a {
//                 span:nth-child(2) {
//                   display: none;
//                 }
//               }

//               ul {
//                 padding-left: 0;

//                 li {
//                   a {
//                     display: inline-block;
//                   }
//                 }
//               }
//             }
//           }

//           .menu-label:not(:last-child) {
//             margin-bottom: 0;
//           }
//         }
//       }
//     }
//   }
// }

// .is-mini-expand {
//   .menu-list a {
//     white-space: nowrap;
//     overflow: hidden;
//     text-overflow: ellipsis;
//   }
// }
</style>
