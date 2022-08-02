<template>
  <card-component
    title="Login"
    icon="lock"
  >
    <router-link
      slot="button"
      to="/"
      class="button is-small"
    >
      Dashboard
    </router-link>

    <form
      method="POST"
      @submit.prevent="submit"
    >
      <b-field label="Username">
        <b-input
          v-model="form.userName"
          name="text"
          type="text"
          required
        />
      </b-field>

      <b-field label="Password">
        <b-input
          v-model="form.password"
          type="password"
          name="password"
          required
        />
      </b-field>

      <!-- <b-field>
        <b-checkbox
          v-model="form.remember"
          type="is-black"
          class="is-thin"
        >
          Remember me
        </b-checkbox>
      </b-field> -->

      <hr>

      <b-field grouped>
        <div class="control">
          <b-button
            native-type="submit"
            type="is-black"
            :loading="isLoading"
          >
            Login
          </b-button>
        </div>
        <!-- <div class="control">
          <router-link
            to="/"
            class="button is-outlined is-black"
          >
            Dashboard
          </router-link>
        </div> -->
      </b-field>
    </form>
  </card-component>
</template>

<script>
import CardComponent from '@/components/CardComponent.vue'
import { login } from "@/api/user";
import {setToken} from '@/utils/auth';
export default {
  name: 'Login',
  components: { CardComponent },
  data () {
    return {
      isLoading: false,
      form: {
        userName: '',
        password: '',
      }
    }
  },
  methods: {
    submit () {
      this.isLoading = true;
      login(this.form)
        .then((response) => {
          if (response.status==200) {
            let userInfo = response.data;
            //console.log(userInfo);
            setToken(userInfo.accessToken);
            this.$store.commit('user', {
              name: userInfo.name,
              email: userInfo.email,
              roleName: userInfo.roleName,
              permissions: userInfo.permissions
            });
            this.$router.push({ name: 'import-data' });
          }else{
            //console.log('login fail');
            setToken('');
            this.$buefy.snackbar.open({
              message: "login failed!",
              queue: false
            });
          }
        })
        .catch((error) => {
          setToken('');
          console.log('login fail');
          console.log(error.response);
          this.$buefy.snackbar.open({
            message: "Login failed!",
            queue: false
          });
        })
        .finally(() => {
          this.isLoading = false;
        });
      // setTimeout(() => {
      //   this.isLoading = false

      //   this.$router.push('/')
      // }, 750)
    }
  }
}
</script>
