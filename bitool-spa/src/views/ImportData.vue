<template>
  <div>
    <section class="section is-main-section">      
    <b-tabs v-model="activeTab">
      <b-tab-item label="Import data" type="is-boxed">
        <!--<b-field class="file is-primary" :class="{ 'has-name': !!file }" >
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
        </b-field>-->  

       <b-field class="file is-primary" :class="{ 'has-name': !!file }" >
          <b-upload multiple v-model="files" class="file-label" @change.native="isShowResult=false; isActiveMessage=false;" 
            accept=".xlsx, .xls, .csv" required validationMessage="Please select correct file type">
            <span class="file-cta">
              <b-icon class="file-icon" icon="upload" ></b-icon>
              <span class="file-label" >Click to upload</span>
            </span>          
          </b-upload>       
        </b-field>

        <div class="is-flex is-flex-direction-column is-justify-content-flex-start is-flex-wrap-wrap" style="width:10px">
          <b-tag v-for="(fileItem, index) in files"
            :key="index"
            type="is-info"
            class="is-flex mb-3" >
            <span class="mr-3">{{fileItem.name}}</span>
            <button class="delete is-small"
              type="button"
              @click="files.splice(index, 1)">
            </button>
          </b-tag>
          <b-autocomplete
            v-show="files.length>0"
            open-on-focus
            v-model="sourceName"
            :data="filteredDataArray"
            placeholder="Assign source..."
            icon-right="magnify"   
            clearable
            size="is-small"              
            @select="option => selected = option"
            :ref="'autocomplete-importdata'">
            <template #empty>No sources found</template>
          </b-autocomplete>
        </div>

        <b-field class="mt-5">
          <b-button type="is-info" @click="importData" :loading="isLoading" :disabled="files.length==0" label="Import Data" 
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
      <b-field class="mt-5"  v-show="mobileNumberList.length>0 &&isShowResultTab2">
        <b-button icon-left="database-import" label="Import clean data" class="mr-3" type="is-info" 
        :disabled="!isEnableImportCleanData" @click="importCleanedMobileNumberList" :loading="isLoadingImportCleanData"/>
      </b-field>
      </b-tab-item>
    </b-tabs> 
    </section>
  </div>
</template>

<script>
import { importCustomerScore, getAdminScores, compareCustomerMobiles,importCleanedMobileNumberList } from "@/api/importData";
import { getSource  } from "@/api/importHistory";
export default {
  name: "Import-Data-view",
  components: {},
  created() {
    this.getSource();
  },
  data() {
    return {
      file: null,
      files:[],
      fileName:'',
      totalRows:0,
      totalErrorRows:0,
      totalImportedRows:0,
      errorList: [],
      adminScores: [],
      sourceName: null,
      sources:[],      
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
      totalRowsTab2:0,
      isLoadingImportCleanData:false,
      isEnableImportCleanData:false
    };
  },
  computed: {
    filteredDataArray() {
      if(!this.sourceName) return this.sources;
      return this.sources.filter((option) => {
          return option
              .toString()
              .toLowerCase()
              .indexOf(this.sourceName.toLowerCase()) >= 0
      })
    }
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
      //formData.append('file', this.file);
      for (let i = 0 ; i < this.files.length ; i++) {
        formData.append('files', this.files[i],this.files[i].name);
      }
      //console.log(this.$store.state.signalRConnectionId);
      if(this.sourceName !== null && this.sourceName.trim() === '') 
        this.sourceName = null;
      importCustomerScore({signalRConnectionId:this.$store.state.signalRConnectionId, sourceName: this.sourceName}, formData)
        .then((response) => {
          if (response.status == 200) {
            this.isEnableImportCleanData = false;
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
          }
        })
        .catch((error) => {
          // this.$buefy.snackbar.open({
          //   message: error,
          //   queue: false,
          //   type: 'is-warning'
          // });
        })
        .finally(() => {
          this.isLoading = false;
          this.files=[];
          this.sourceName = null;
        });
    },
    compareCustomerMobiles() {
      this.isLoadingTab2 = true;
      this.isShowResultTab2=false;
      this.isEnableImportCleanData=false; 
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
              if(this.totalNewCustomers)
                this.isEnableImportCleanData=true;             
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
    importCleanedMobileNumberList() {     
      this.isLoadingImportCleanData=true;  
      importCleanedMobileNumberList(
        {fileName: this.fileNameTab2, signalRConnectionId:this.$store.state.signalRConnectionId}, 
        this.mobileNumberList)
        .then((response) => {
          if (response.status == 200) {
            this.isEnableImportCleanData = false;
            var data = response.data;
            if(data){
              // this.errorList= data.errorList;
              // this.totalRows= data.totalRows;
              // this.totalErrorRows= this.errorList.length;
              // this.totalImportedRows= this.totalRows- this.totalErrorRows;
              if(!data.shouldSendEmail){
                 this.$buefy.snackbar.open({
                  message: `Import ${this.fileNameTab2} successfully!`,
                  queue: false,
                });
              }else{
                this.$buefy.snackbar.open({
                  message: `Import ${this.fileNameTab2} successfully!\nSystem will send email to inform when the new export data is ready`,
                  queue: false,
                  duration: 6000
                });
              }
            }  
          }
        })
        .catch((error) => {
          // this.$buefy.snackbar.open({
          //   message: error,
          //   queue: false,
          //   type: 'is-warning'
          // });
        })
        .finally(() => {
          // this.isLoadingTab2 = false;
          // this.fileTab2=null;
          this.isLoadingImportCleanData=false;
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
    getSource() {
      getSource()
        .then((response) => {
          if (response.status == 200) {
            this.sources = response.data;
          }else{
            console.log(response);
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          //this.isLoading = false;
        });
    }    
  }
};
</script>
