<template>
  <card-component
    title="Update password"
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


      <b-field label="Current password">
        <b-input
          v-model.trim="form.currentPassword"
          type="password"
          name="password"
          required
        />
      </b-field>

      <b-field :type="validPasswordMessages.length>0? 'is-danger':''" :message="validPasswordMessages">
        <template #label>
          New password
          <b-tooltip type="is-dark" position="is-right" multilined>
              <b-icon size="is-small" icon="help-circle-outline"></b-icon>
              <template v-slot:content>
                <div class="has-text-left">
                  <p class="is-size-7">Password must contain the following:</p>
                  <div class="content">
                    <ul class="is-size-7">
                      <li>A <b>lowercase</b> letter</li>
                      <li>A <b>capital (uppercase)</b> letter</li>
                      <li>A <b>special</b> letter (!@#$%^...)</li>
                      <li>A <b>number</b></li>
                      <li>Minimum <b>8 characters</b></li>
                    </ul>              
                  </div>                                  
                </div>
              </template>
          </b-tooltip>
        </template>
        <b-input
          v-model.trim="form.newPassword"
          type="password"
          name="password"
          required
          minlength="8"
          maxlength="30"
        />
      </b-field>

      <b-field label="Confirm password" :type="validConfirmPasswordMessages.length>0? 'is-danger':''" :message="validConfirmPasswordMessages">
        <b-input
          v-model.trim="form.confirmPassword"
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
            type="is-info"
            :loading="isLoading"
          >
            Update password
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
import _ from 'lodash';
import CardComponent from '@/components/CardComponent.vue'
import { changePassword } from "@/api/user";
import {setToken} from '@/utils/auth';
export default {
  name: 'Login',
  components: { CardComponent },
  data () {
    return {
      isLoading: false,
      form: {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      },
      // has_minimum_lenth: false,
      // has_number: false,
      // has_lowercsae: false,
      // has_uppercase: false,
      // has_special: false,
      validPasswordMessages:[],
      validConfirmPasswordMessages:[]
    }
  },
  watch:{
    "form.newPassword":_.debounce(function(value){
      this.validPasswordMessages= this.checkValidPassword(value); 
    },800),
    "form.confirmPassword":_.debounce(function(value){
      if(value!== this.form.newPassword)
        this.validConfirmPasswordMessages=['New password and confirmation password do not match'];
      else 
        this.validConfirmPasswordMessages=[];
    },1000)
  },
  methods: {
    submit () {
      if(this.form.currentPassword === this.form.newPassword){
        this.$buefy.snackbar.open({
          message: 'Current password and new password should not be the same',
          queue: false,
          duration: 2000,
          type: 'is-warning'
        });
        return;
      }

      this.validPasswordMessages= this.checkValidPassword(this.form.newPassword);
      if(this.validPasswordMessages.length>0){
        this.$buefy.snackbar.open({
          message: 'Please check new password!',
          queue: false,
          duration: 1000,
          type: 'is-danger'
        });
        return;
      }

      if(this.form.confirmPassword !== this.form.newPassword){
        this.$buefy.snackbar.open({
          message: 'Please check confirm password!',
          queue: false,
          duration: 2000,
          type: 'is-danger'
        });
        return;
      }
      
      this.isLoading = true;     
      changePassword(this.form)
        .then((response) => {
          if (response.status==200) {
            this.$buefy.snackbar.open({
              message: 'Changed password successfully',
              queue: false,
              duration: 2000              
            });
            this.$router.push({ name: 'login' });
          }else if (response.status==401) {
            //setToken('');
            this.$buefy.snackbar.open({
              message: "The current password is incorrect",
              queue: false
            });
          }
          else{
            //setToken('');
            this.$buefy.snackbar.open({
              message: "Can not change this password",
              queue: false
            });
          }
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
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
