<template>
  <div>
    <section class="section is-main-section">
      <tiles>
       <!-- <profile-update-form class="tile is-child" /> -->
        <card-component
          title="Profile"
          icon="account"
          class="tile is-child"
        >
          <!-- <user-avatar class="image has-max-width is-aligned-center" />
          <hr> -->
          <b-field label="Name">
            <b-input
              :value="userName"
              custom-class="is-static"
              readonly
            />
          </b-field>
          <hr>
          <b-field label="E-mail">
            <b-input
              :value="userEmail"
              custom-class="is-static"
              readonly
            />
          </b-field>
        </card-component>
      </tiles>
      <card-component
      title="Change Password"
      icon="lock"
  >
    <form @submit.prevent="submit" style="width:1000px">
      <b-field
        horizontal
        label="Current password"
        message="Required. Your current password"
      >
        <b-input
          v-model="form.currentPassword"
          name="currentPassword"
          type="password"
          required
          autcomplete="currentPassword"
        />
      </b-field>
      <hr>
      <b-field
        horizontal
        label="New password"
        message="Required. New password"
      >
        <b-input
          v-model="form.newPassword"
          name="newPassword"
          type="password"
          required
          autocomplete="newPassword"
        />
      </b-field>
      <b-field
        horizontal
        label="Confirm password"
        message="Required. New password one more time"
      >
        <b-input
          v-model="form.confirmNewPassword"
          name="confirmNewPassword"
          type="password"
          required
          autocomplete="confirmNewPassword"
        />
      </b-field>
      <hr>
      <b-field horizontal>
        <div class="control">
          <b-button
            native-type="submit"
            type="is-info"
            :loading="isLoading"
          >
            Submit
          </b-button>
        </div>
      </b-field>
    </form>
  </card-component>
    </section>
  </div>
</template>

<script>
import { mapState } from 'vuex'
import CardComponent from '@/components/CardComponent.vue'
import Tiles from '@/components/Tiles.vue'
import UserAvatar from '@/components/UserAvatar.vue'
import { changePassword } from "@/api/user";
export default {
  name: 'Profile',
  components: {
    UserAvatar,
    Tiles,
    CardComponent
  },
  data () {
    return {
       form:{
         currentPassword: null,
         newPassword:null,
         confirmNewPassword:null
       },
       isLoading:false
    }
  },
  computed: {
    ...mapState([
      'userName',
      'userEmail'
    ])
  },
  methods:{
    submit() {
      this.isLoading = true;
      if (this.form.newPassword !== this.form.confirmNewPassword) {
          this.$buefy.snackbar.open({
            message: 'New password and confirm password do not match',
            queue: false,
            type: 'is-warning'
          });
          this.isLoading = false;
          return;
        }
      changePassword(this.form)
        .then((response) => {
          if (response.status == 200) {
            this.$buefy.snackbar.open({
              message: "Changed password successfully",
              queue: false,
            });
           this.$router.push({ name: "login" });            
          }
          else{
            this.$buefy.snackbar.open({
            message: 'Current password is not correct',
            queue: false,
            type: 'is-warning'
          }); }  
        })
        .catch((error) => {
           this.$buefy.snackbar.open({
            message: 'Current password is not correct',
            queue: false,
            type: 'is-danger'
          });
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
  }
}
</script>
