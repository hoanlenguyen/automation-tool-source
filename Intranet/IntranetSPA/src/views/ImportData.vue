<template>
  <div>
    <section class="section is-main-section">      
    <b-tabs v-model="activeTab">
      <b-tab-item label="Import data" type="is-boxed">
        <b-field class="file is-primary" :class="{ 'has-name': !!file }" >
        <b-upload v-model="file" class="file-label" @change.native="isShowResult=false; isActiveMessage=false; fileName=file?file.name:''" 
          accept=".xlsx, .xls, .csv" required validationMessage="Please select correct file type">
          <span class="file-cta">
            <b-icon class="file-icon" icon="upload" ></b-icon>
            <span class="file-label" >Click to upload</span>
          </span>
          <span class="file-name" v-if="file">
            {{ file.name }}
          </span>
        </b-upload>
      </b-field>
      <b-field class="mt-5">
        <b-button type="is-info" @click="importData" :loading="isLoading" :disabled="!file" label="Import Data" 
          icon-left="database-import"/>
      </b-field>
      <b-field class="mt-5" v-show="isShowResult">
        <h5 class="subtitle is-6">Import {{fileName}} successfully!</h5>
      </b-field>
       <div class="mt-5" v-show="isShowResult">
        <h5 class="subtitle is-6" >Total rows: <strong>{{totalRows}}</strong></h5>
        <h5 class="subtitle is-6" >Total imported rows: <strong>{{totalImportedRows}}</strong></h5>
        <h5 class="subtitle is-6" >Total error rows: <strong>{{totalErrorRows}}</strong></h5>
      </div>
      <b-field class="mt-5"  v-show="errorList.length>0 &&isShowResult">
        <b-button label="Download error list" class="mr-3" type="is-primary" @click="downloadErrorListExcel"/>
      </b-field>
      </b-tab-item>

      <b-tab-item label="Cleaning data" type="is-boxed">
        <b-field class="file is-primary" :class="{ 'has-name': !!fileTab2 }" >
        <b-upload v-model="fileTab2" class="file-label" @change.native="isShowResultTab2=false; fileNameTab2=fileTab2?fileTab2.name:''" 
          accept=".xlsx, .xls, .csv" required validationMessage="Please select correct file type">
          <span class="file-cta">
            <b-icon class="file-icon" icon="upload" ></b-icon>
            <span class="file-label" >Click to upload</span>
          </span>
          <span class="file-name" v-if="fileTab2">
            {{ fileTab2.name }}
          </span>
        </b-upload>
      </b-field>
      <b-field class="mt-5">
        <b-button type="is-info" @click="compareCustomerMobiles" :loading="isLoadingTab2" :disabled="!fileTab2" label="Clean data" 
          icon-left="database-import"/>
      </b-field>
      <b-field class="mt-5" v-show="isShowResultTab2">
        <h5 class="subtitle is-6">Check {{fileNameTab2}} successfully!</h5>
      </b-field>
       <div class="mt-5" v-show="isShowResultTab2">
        <h5 class="subtitle is-6" >Total rows: <strong>{{totalRowsTab2}}</strong></h5>
        <h5 class="subtitle is-6" >Total new customer mobile Nos: <strong>{{totalNewCustomers}}</strong></h5>
      </div>
      <b-field class="mt-5"  v-show="mobileNumberList.length>0 &&isShowResultTab2">
        <b-button label="Download customer mobile list" class="mr-3" type="is-primary" @click="downloadMobileNumberListExcel"/>
      </b-field>
      </b-tab-item>
    </b-tabs> 
    </section>
  </div>
</template>

<script>
//import moment from "moment";
import { importCustomerScore, getAdminScores, compareCustomerMobiles } from "@/api/importData";
export default {
  name: "ImportData",
  components: {},
  data() {
    return {
      file: null,
      fileName:'',
      totalRows:0,
      totalErrorRows:0,
      totalImportedRows:0,
      errorList: [],
      adminScores: [],      
      isLoadProcessExcel: false,
      isLoading: false,
      isShowResult:false,
      activeTab: 0,
      fileTab2: null,
      fileNameTab2:'',
      isShowResultTab2:false,
      isLoadingTab2:false,
      mobileNumberList:[],
      totalNewCustomers:0,
      totalRowsTab2:0
    };
  },
  watch: {},
  methods: {
    getAdminScoreList() {
      getAdminScores()
        .then((response) => {
          if (response.status == 200) {
            this.adminScores = response.data;             
          } else if (response.status == 401) {
            this.$router.push({ name: "login" });
          }
        })
        .catch((error) => {
        })
        .finally(() => {
        });
    },     
    importData() {
      this.isLoading = true;
      this.isShowResult=false;
      this.isActiveMessage= false;
      let formData = new FormData();
      formData.append('file', this.file);
      console.log(this.$store.state.signalRConnectionId);
      importCustomerScore({signalRConnectionId:this.$store.state.signalRConnectionId}, formData)
        .then((response) => {
          if (response.status == 200) {
            this.isShowResult = true;
            var data = response.data;
            if(data){
              this.errorList= data.errorList;
              this.totalRows= data.totalRows;
              this.totalErrorRows= this.errorList.length;
              this.totalImportedRows= this.totalRows- this.totalErrorRows;
              if(!data.shouldSendEmail){
                 this.$buefy.snackbar.open({
                  message: `Import ${this.fileName} successfully!`,
                  queue: false,
                });
              }else{
                this.$buefy.snackbar.open({
                  message: `Import ${this.fileName} successfully!\nSystem will send email to inform when the new export data is ready`,
                  queue: false,
                  duration: 6000
                });
              }
            }            
           
            // var counter = 0;
            // var looper = setInterval(function(){ 
            // counter++;
            // console.log("Counter is: " + counter);
            // if (counter >= 5)
            // {
            //     clearInterval(looper);
            // }}, 1000);
          }
        })
        .catch((error) => {
          this.$buefy.snackbar.open({
            message: error,
            queue: false,
            type: 'is-warning'
          });
        })
        .finally(() => {
          this.isLoading = false;
          this.file=null;
        });
    },
    compareCustomerMobiles() {
      this.isLoadingTab2 = true;
      this.isShowResultTab2=false;
      let formData = new FormData();
      formData.append('file', this.fileTab2);
      compareCustomerMobiles(formData)
        .then((response) => {
          if (response.status == 200) {
            this.isShowResultTab2 = true;
            const data = response.data;
            if(data){
              this.mobileNumberList= data.mobileNumberList;
              this.totalRowsTab2= data.totalRows;
              this.totalNewCustomers= this.mobileNumberList.length;             
            }            
            this.$buefy.snackbar.open({
              message: `Check ${this.fileNameTab2} successfully!`,
              queue: false,
            });}
        })
        .catch((error) => {
          this.$buefy.snackbar.open({
            message: error,
            queue: false,
            type: 'is-warning'
          });
        })
        .finally(() => {
          this.isLoadingTab2 = false;
          this.fileTab2=null;
        });
    },
    downloadErrorListExcel() {
      console.log("downloadErrorListExcel");
      if (this.errorList.length > 0) 
         this.exportExcelData(this.errorList, "ErrorList", 30);
    },
    downloadMobileNumberListExcel() {
      console.log("downloadMobileNumberListExcel");
      if (this.mobileNumberList.length > 0) {
        // let mobileList = this.mobileNumberList.map((p) => ({
        //   CustomerMobileNo: p,
        // }));
        this.exportExcelData(this.mobileNumberList, "CustomerMobileNoList", 30, false);
      }
    },
    checkValidPhoneNumber(input) {
      if(!input) return false;
      if (input.length != 9) return false;
      var firstChar = input[0];
      if (firstChar != "6" && firstChar != "8" && firstChar != "9")
        return false;
      const parsed = parseInt(input);
      return !isNaN(parsed);
    }
  }
};
</script>
