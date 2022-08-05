<template>
  <section class="section is-main-section">
    <b-table
      :data="data"
      :loading="isLoading"
      mobile-cards
      scrollable
      backend-pagination
      :total="totalItems"
      :per-page="filter.rowsPerPage"      
      :pagination-simple="false"
      pagination-position="bottom"
      
      backend-sorting
      :default-sort="filter.sortBy"
      :default-sort-direction="filter.sortDirection"
      :sort-icon="sortIcon"
      :sort-icon-size="sortIconSize"
      @sort="onSort"

      aria-next-label="Next page"
      aria-previous-label="Previous page"
      aria-page-label="Page"
      aria-current-label="Current page"           
      :pagination-order="paginationOrder"   
      :debounce-page-input="200"
    >
      <b-table-column
        field="Name"
        label="Name"
        sortable        
        width="200px"
        sticky
        centered
        v-slot="props"
      >       
      {{props.row.name}}
      </b-table-column>

      <b-table-column
        field="employeeCode"
        label="Employee code"
        sortable        
        width="150px"
        v-slot="props"
      >       
      {{props.row.employeeCode}}
      </b-table-column>

      <b-table-column
        field="rankId"
        label="Rank"
        width="150px"       
        v-slot="props"
      >       
      {{props.row.rank}}
      </b-table-column>
       
      <b-table-column
        field="dept"
        label="Department"
        width="150px"       
        v-slot="props"
      >       
      {{props.row.dept}}
      </b-table-column>

       <b-table-column
        field="dept"
        label="Brand"
        width="200px"       
        v-slot="props"
      >       
      {{props.row.brand}}
      </b-table-column>


      <b-table-column
        field="bankName"
        label="BankName"
        width="200px"       
        v-slot="props"
      >       
      {{props.row.bankName}}
      </b-table-column>

      <b-table-column
        field="bankAccountNumber"
        label="Bank Account Number"
        width="150px"       
        v-slot="props">       
        {{props.row.bankAccountNumber}}
      </b-table-column>

      <b-table-column
        field="idNumber"
        label="Id Number"
        width="150px"       
        v-slot="props">       
        {{props.row.idNumber}}
      </b-table-column>

      <b-table-column
        field="salary"
        label="Salary"
        width="150px"       
        v-slot="props">       
        {{props.row.salary|formattedNumber}}
      </b-table-column>

      <b-table-column
        field="Status"
        label="Status"
        sortable
        v-slot="props"
        width="120px">        
       <span :class="props.row.status?'':'has-text-danger'">{{ props.row.status?'Active':'Inactive' }}</span>        
      </b-table-column>

      <b-table-column
        field="startDate"
        label="Start Date"
        sortable
        v-slot="props"
        width="120px">
       {{ props.row.startDate | dateTime('DD-MMM-YYYY') }} 
      </b-table-column>

      <b-table-column
        field="birthDate"
        label="Birth Date"
        sortable
        v-slot="props"
        width="120px">
       {{ props.row.birthDate | dateTime('DD-MM-YYYY') }} 
      </b-table-column>

      <b-table-column
        field="CreationTime"
        label="Creation Time"
        sortable
        v-slot="props"
        width="120px">
       {{ props.row.creationTime | dateTime }} 
      </b-table-column>

      <b-table-column
        field="LastModificationTime"
        label="Last Update Time"
        v-slot="props"
        width="120px">
       {{ props.row.lastModificationTime | dateTime }} 
      </b-table-column>

      <b-table-column
        field="lastModifierUser"
        label="Last Update By"
        v-slot="props"
        width="200px">
       {{ props.row.lastModifierUser }} 
      </b-table-column>

      <b-table-column
        field="Edit"
        label="Edit"        
        v-slot="props"
        width="150px">        
        <b-button 
          v-if="canUpdate"
          title="edit"          
          class="button mr-5"

          @click="editModel(props.row)" 
          style="padding: 0; border: none; background: none;">
          <b-icon
          size="is-small"
            icon="pencil"
            type="is-info">
          </b-icon>
        </b-button> 
        <b-button
          v-if="canDelete" 
          title="delete"          
          class="button has-text-grey"
          @click="deleteSelectedModel(props.row.id)" 
          style="padding: 0; border: none; background: none;">
          <b-icon
             size="is-small"
            icon="delete">
          </b-icon>
        </b-button>       
      </b-table-column>

      <template #empty>
        <div class="has-text-centered">No records</div>
      </template>
      <div slot="subheading" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">header
      </div>
      <div slot="footer" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">
        <b-button
          v-if="canCreate"
          label="Import"
          type="is-primary"
          class="mr-4"
          icon-left="note-plus"
          @click="isModalImportActive=true"
          :loading="isImportLoading"          
        />
        <b-button
          v-if="canCreate"
          label="Create"
          type="is-info"
          class="mr-4"
          icon-left="note-plus"
          @click="isModalActive=true"
        />
        <b-button
            label="Reset"
            type="is-light"
            class="mr-4"
            icon-left="reload"
            @click="resetFilter"
        />
        <span class="has-text-weight-normal mr-4">Total count: {{totalItems}}</span>
        <b-select v-model="filter.rowsPerPage"  @input="onChangePageSize" class="mr-4">
            <option v-for="i in pageOptions" :value="i" :key="i">{{`${i} per page`}}</option>        
        </b-select>
        <b-pagination
            :total="totalItems"
            v-model="filter.page"
            :range-before="1"
            :range-after="1"
            :order="`is-right`"
            :size="``"
            :simple="false"
            :rounded="false"
            :per-page="filter.rowsPerPage"
            :icon-prev="`chevron-left`"
            :icon-next="`chevron-right`"
            aria-next-label="Next page"
            aria-previous-label="Previous page"
            aria-page-label="Page"
            aria-current-label="Current page"
            :page-input="true"
            :page-input-position="``"
            :debounce-page-input="``"
            @change="onChangePageNumber">
        </b-pagination>        
      </div>
    </b-table>
    <b-modal v-model="isModalActive" trap-focus has-modal-card full-screen :can-cancel="false" scroll="keep">
      <div class="modal-card">
        <header class="modal-card-head">
          <p class="modal-card-title">{{model.id==0?'Create':'Update'}}</p>                 
        </header>
        <section class="modal-card-body">
          <b-field>
            <b-switch v-model="model.status" type='is-info'>{{model.status?'Active':'Inactive'}}</b-switch>
          </b-field>
          <div class="columns">
            <b-field label="Name" class="column is-3">
              <b-input
                type="Text"
                v-model="model.name"
                required>
              </b-input>
            </b-field>

            <b-field label="Employee Code" class="column is-3">
              <b-input
                type="Text"
                v-model="model.employeeCode"
                required>
              </b-input>
            </b-field>

            <b-field label="Id Number" class="column is-3">
              <b-input
                type="Text"
                v-model="model.idNumber"
                required>
              </b-input>
            </b-field>

            <b-field label="Birth Date" class="column is-3">
              <b-datepicker
              icon="calendar-today"
              locale="en-SG"
              v-model="birthDate"
              editable>
              </b-datepicker>
            </b-field>           
          </div> 

        <div class="columns">
          <b-field label="Rank" class="column is-3">
              <b-select
                placeholder="Select role"
                v-model="model.rankId"
                clearable
                expanded
              >
              <option
                v-for="option in ranks"
                :value="option.id"
                :key="option.id"
              >
                {{ option.name }}
              </option>
              </b-select>
          </b-field>

          <b-field label="Department" class="column is-3">
            <b-select
              placeholder="Select department"
              v-model="model.deptId"
              clearable
              expanded
            >
            <option
              v-for="option in departments"
              :value="option.id"
              :key="option.id"
            >
              {{ option.name }}
            </option>
            </b-select>
          </b-field>

          <b-field label="Bank" class="column is-3">
            <b-select
              placeholder="Select bank"
              v-model="model.bankId"
              clearable
              expanded
            >
            <option
              v-for="option in banks"
              :value="option.id"
              :key="option.id"
            >
              {{ option.name }}
            </option>
            </b-select>
          </b-field>

          <b-field label="Bank Account Number" class="column is-3">
            <b-input
              type="Text"
              v-model="model.bankAccountNumber">
            </b-input>
          </b-field>
        </div>

        <div class="columns">
          <b-field label="Salary" class="column is-3">
            <b-input
              type="number"
              v-model="model.salary"
              required>
            </b-input>
          </b-field>

          <b-field label="Start Date" class="column is-3">
              <b-datepicker
              icon="calendar-today"
              locale="en-SG"
              v-model="startDate"
              editable>
              </b-datepicker>
          </b-field>

          <b-field label="Backend User" class="column is-3">
            <b-input
              type="Text"
              v-model="model.backendUser"
              required>
            </b-input>
          </b-field>

          <b-field label="Backend Pass" class="column is-3">
            <b-input
              type="Text"
              v-model="model.backendPass"
              required>
            </b-input>
          </b-field>
        </div>

        <div class="columns">
          <b-field label="Brand" class="column is-3">
            <multiselect
            v-model="selectBrands"
            tag-placeholder=""
            placeholder="Select brand"             
            :options="brands"
            label="name"
            track-by="id"
            :multiple="true"
            :taggable="true"
            selectLabel="Add"
            deselectLabel="Remove"
          >
          </multiselect>
          </b-field>

          <b-field label="Intranet Role" class="column is-3">
            <b-select
              placeholder="Select role"
              v-model="model.roleId"
              clearable
              expanded
            >
            <option
              v-for="option in roles"
              :value="option.id"
              :key="option.id"
            >
              {{ option.name }}
            </option>
            </b-select>
          </b-field>

          <b-field label="Note" class="column is-6">
            <b-input
              type="Text"
              v-model="model.note">
            </b-input>
          </b-field>          
        </div>        
        </section>
        <footer class="modal-card-foot">
          <b-button label="Close" @click="cancelCreateOrUpdate" />
          <b-button :label="model.id==0?'Create':'Update'"type="is-primary" @click="createOrUpdateModel"/>
        </footer>
        </div>
    </b-modal>
    <b-modal v-model="isModalImportActive" trap-focus has-modal-card :can-cancel="false" width="1200" scroll="keep">
      <div class="modal-card" style="height:500px">
        <header class="modal-card-head">
          <p class="modal-card-title">Import employees</p>                 
        </header>
        <section class="modal-card-body">
          <b-field class="file is-primary" :class="{ 'has-name': !!file }" >
           <b-upload v-model="file" class="file-label" @change.native="isShowImportResult=false; fileName=file?file.name:''" 
            accept=".xlsx, .xls, .csv" required validationMessage="Please select correct file type">
            <span class="file-cta">
              <b-icon class="file-icon" icon="upload" ></b-icon>
              <span class="file-label" >Click to upload</span>
            </span>
            <span class="file-name" v-if="file">
              {{ fileName }}
            </span>
          </b-upload>
        </b-field> 
        <b-field class="mt-5" v-show="isShowImportResult">
          <h5 class="subtitle is-6">Import {{fileName}} successfully!</h5>
        </b-field>
        <div class="mt-5" v-show="isShowImportResult">
          <h5 class="subtitle is-6" >Total rows: <strong>{{totalRows}}</strong></h5>
          <h5 class="subtitle is-6" >Total imported rows: <strong>{{totalImportedRows}}</strong></h5>
          <h5 class="subtitle is-6" >Total error rows: <strong>{{totalErrorRows}}</strong></h5>
        </div>
        <b-field class="mt-5"  v-show="errorList.length>0 &&isShowImportResult">
          <b-button label="Download error list" class="mr-3" type="is-primary" @click="downloadErrorListExcel"/>
        </b-field>                
        </section>
        <footer class="modal-card-foot">
          <b-button label="Cancel" @click="file=null;isShowImportResult=false;isModalImportActive=false" />
          <b-button label="Import Data" type="is-primary" :disabled="file==null" @click="importEmployees"/>
        </footer>
        </div>
    </b-modal>

    <b-modal v-model="isDeleteModalActive" trap-focus has-modal-card auto-focus :can-cancel="false" scroll="keep">
      <div class="modal-card" style="width:300px">
        <header class="modal-card-head">
          <p class="modal-card-title">Are you sure to delete this data</p>                 
        </header>
        <footer class="modal-card-foot">
          <b-button label="Close" @click="isDeleteModalActive=false; selectedId=null" />
          <b-button label="Confirm" type="is-info" @click="deleteData"/>
        </footer>
        </div>
    </b-modal>
  </section>
</template>
<script>
import moment from "moment";
import Multiselect from "vue-multiselect";
import { getDetail, getList, createOrUpdate, deleteData, importEmployees  } from "@/api/employee";
import { getDropdown as getRoleDropdown } from "@/api/role";
import { getDropdown as getBankDropdown } from "@/api/bank";
import { getDropdown as getBrandDropdown } from "@/api/brand";
import { getDropdown as getDepartmentDropdown } from "@/api/department";
import { getDropdown as getRankDropdown } from "@/api/rank";
export default {
  name:"employee",
  components: { Multiselect },
  created() {
    this.getList();
    this.getRoleDropdown();
    this.getDepartmentDropdown();
    this.getBankDropdown();
    this.getBrandDropdown();
    this.getRankDropdown();
  },
  data() {
    return {
      file: null,
      fileName:'',
      isShowImportResult:false,
      isImportLoading:false,
      errorList:[],
      totalRows:0,
      totalImportedRows:0,
      totalErrorRows:0,
      data: [],
      totalItems:0,
      isLoading:false,
      isPaginationSimple: false,
      isPaginationRounded: false,
      paginationPosition: "bottom",
      defaultSortDirection: "desc",
      sortIcon: "arrow-up",
      sortIconSize: "is-small",
      currentPage: 1,
      hasInput: false,
      paginationOrder: "is-right",
      inputPosition: "is-input-left",
      inputDebounce: "",
      pageOptions:[20,50,100],
      importTimeFrom:null,
      importTimeTo:null,
      filter:{
        page:1,
        rowsPerPage:20,
        sortBy:'Id',
        sortDirection:'desc',
        keyword:null,
        status:null
      },
      defaultFilter:{
        page:1,
        rowsPerPage:20,
        sortBy:'Id',
        sortDirection:'desc',
        keyword:null,
        status:null
      },
      model:{
        name:null,
        status:true,
        id:0
      },
      defaultModel:{
        name:null,
        status:true,
        id:0
      },
      isModalActive:false,
      isModalImportActive:false,
      isDeleteModalActive:false,
      selectedId:null,
      roles:[],
      departments:[],
      banks:[],
      brands:[],
      ranks:[],
      birthDate:null,
      startDate:null,
      selectBrands:[]
    };
  },
  watch: {},
  computed: {
    canCreate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Permissions.Employee.Create"
        )
      );
    },
    canUpdate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Permissions.Employee.Update"
        )
      );
    },
    canDelete() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Permissions.Employee.Delete"
        )
      );
    }
   },
  methods: {
    resetFilter() {
      this.filter = { ...this.defaultFilter };       
      this.getList();
    },
    onChangePageSize(){
      this.filter.page = 1;
      this.getList();
    },
    onChangePageNumber(page){
      this.getList();
    }, 
    onSort(field, order) {
      this.filter.sortBy = field
      this.filter.sortDirection = order
      this.getList();
    },
    getList() {
      this.isLoading = true;     

      getList(this.filter)
        .then((response) => {
          if (response.status == 200 && response.data) {
            const result =  response.data;
            this.totalItems = result.totalItems;
            this.data = result.items;             
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    closeModalDialog(){
      this.model= {...this.defaultModel};
      this.birthDate = null;
      this.startDate = null;
      this.selectBrands= [];
      this.isModalActive= false;
    },
    cancelCreateOrUpdate(){
      this.closeModalDialog();
    },
    createOrUpdateModel(){
      if(this.birthDate)
        this.model.birthDate = `${this.birthDate.getFullYear()}-${('0' + (this.birthDate.getMonth()+1)).slice(-2)}-${('0' + this.birthDate.getDate()).slice(-2)}`;

      if(this.startDate)
        this.model.startDate = `${this.startDate.getFullYear()}-${('0' + (this.startDate.getMonth()+1)).slice(-2)}-${('0' + this.startDate.getDate()).slice(-2)}`;
      
      this.model.brandIds = this.selectBrands.length>0? this.selectBrands.map((p) => p.id).join():null;       
      console.log('this.model.brandIds' + this.model.brandIds);
      createOrUpdate(this.model)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `${this.model.id==0?'Create':'Update'} successfully!`,
              queue: false,
            });
          }
        })
      .catch((error) => {})
      .finally(() => {
        this.closeModalDialog();
        this.getList();
      });
    },
    editModel(input){
      this.model= {...input};
      this.birthDate= moment(this.model.birthDate,'YYYY-MM-DD').toDate();
      this.startDate= moment(this.model.startDate,'YYYY-MM-DD').toDate();
      console.log('brandIds '+this.model.brandIds);
      if(this.model.brandIds){
        let brandIds= this.model.brandIds.split(',').map(Number);
        //console.log('brandIds array '+ brandIds);
        this.selectBrands= this.brands.filter(p=> brandIds.includes(p.id));
        //console.log('selectBrands '+  this.selectBrands);
      }

      this.isModalActive= true;
    },
    deleteData(){
      deleteData(this.selectedId)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `Delete data successfully!`,
              queue: false,
            });
          }
        })
      .catch((error) => {})
      .finally(() => {
        this.isDeleteModalActive=false;
        this.selectedId= null;
        this.getList();
      });
    },
    deleteSelectedModel(id){
      if(id){
        this.isDeleteModalActive=true;
        this.selectedId= id;
      }
    },
    importEmployees(){
      this.isImportLoading=true;
      this.isShowImportResult=false;
      this.errorList=[];
      let formData = new FormData();
      formData.append('file', this.file);
      importEmployees({},formData)
      .then((response) => {
          if (response.status == 200) {
            var data = response.data;
            if(data){
              this.errorList= data.errorList;
              this.totalRows= data.totalRows;
              this.totalErrorRows= this.errorList.length;
              this.totalImportedRows= this.totalRows- this.totalErrorRows;
              this.isShowImportResult=true;
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
              this.getList();
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
          this.isImportLoading = false;
          this.file=null;
        });
    },
    downloadErrorListExcel() {
      console.log("downloadErrorListExcel");
      if (this.errorList.length > 0) 
         this.exportExcelData(this.errorList, "ErrorList", 30);
    },
    getRoleDropdown() {
      getRoleDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.roles = response.data;
          } 
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
        });
    },
    getDepartmentDropdown() {
      getDepartmentDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.departments = response.data;
          } 
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
        });
    },
    getBankDropdown() {
      getBankDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.banks = response.data;
          } 
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
        });
    },
    getBrandDropdown() {
      getBrandDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.brands = response.data;
          } 
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
        });
    },
    getRankDropdown() {
      getRankDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.ranks = response.data;
          } 
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
        });
    },

  }
};
</script>
<style >
</style>
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>