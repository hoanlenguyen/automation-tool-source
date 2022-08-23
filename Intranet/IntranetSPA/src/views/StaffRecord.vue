<template>
  <section class="section is-main-section">
    <b-table
      :data="data"
      :loading="isLoading"
      
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
        field="DepartmentId"
        label="Dept"
        sortable        
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.department}}
      </b-table-column>

      <b-table-column
        field="EmployeeId"
        label="Employee Name"
        v-slot="props"
        width="300px"
        header-class="is-size-7">        
       {{ props.row.employeeName}}       
      </b-table-column>

      <b-table-column
        field="EmployeeId"
        label="Employee Code"
        v-slot="props"
        width="300px"
        header-class="is-size-7">        
       {{ props.row.employeeCode}}       
      </b-table-column>

      <b-table-column
        field="recordType"
        label="Record type"
        v-slot="props"
        width="300px"
        header-class="is-size-7">        
       {{ props.row.recordTypeName}}       
      </b-table-column>

      <b-table-column
        field="startDate"
        label="Start Date"
        sortable
        v-slot="props"
        width="300px"
        header-class="is-size-7">
       {{ props.row.startDate | dateTime('DD-MM-YYYY') }} 
      </b-table-column>

      <b-table-column
        field="endDate"
        label="End Date"
        sortable
        v-slot="props"
        width="300px"
        header-class="is-size-7">
       {{ props.row.endDate | dateTime('DD-MM-YYYY') }} 
      </b-table-column>

      <b-table-column
        field="creationTime"
        label="Creation Time"
        sortable
        v-slot="props"
        width="300px"
        header-class="is-size-7">
       {{ props.row.creationTime | dateTime }} 
      </b-table-column>

      <b-table-column
        field="Edit"
        label="Edit"        
        v-slot="props"
        width="100px"
        header-class="is-size-7">
        <b-button 
          v-if="canUpdate"
          title="edit"          
          class="button mr-5"
          @click="getDetail(props.row.id)"
          style="padding: 0; border: none; background: none;">
          <b-icon
            icon="pencil"
            type="is-info">
          </b-icon>
        </b-button> 
        <!-- <b-button 
          v-if="canDelete"
          title="delete"          
          class="button has-text-grey"
          @click="deleteSelectedModel(props.row.id)" 
          style="padding: 0; border: none; background: none;">
          <b-icon
            icon="delete">
          </b-icon>
        </b-button>        -->
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
            label="Create"
            type="is-info"
            class="mr-4"
            icon-left="note-plus"
            @click="isModalActive=true"
            v-if="canCreate"
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
    <b-modal v-model="isModalActive" trap-focus has-modal-card :can-cancel="false" scroll="keep">
      <div class="modal-card">
        <header class="modal-card-head">
          <p class="modal-card-title">{{model.id==0?'Create':'Update'}}</p>                 
        </header>
        <section class="modal-card-body">
          <b-field label="Name of staff">             
             <b-autocomplete
              open-on-focus
              v-model="searchEmployee"
              :data="filterEmployees"
              field="fullName"
              placeholder="Search employee..."                           
              clearable
              size="is-small"
              @typing="getAsyncData"              
              @select="option => {selected = option; model.employeeId=option!==null? option.id:0}">
              <template #empty>No employees found</template>
            </b-autocomplete>
          </b-field>

          <b-field label="Department"> 
            <div class="is-flex is-flex-direction-column">
              <div v-for="(item, index) in departments" :key="index">
              <b-radio  v-model="model.departmentId"
                :native-value="item.id"
                type="is-info">
                {{item.name}}
              </b-radio>
            </div>
             <b-input
              type="Text"
              v-if="model.departmentId==0"
              v-model="model.otherDepartment">
            </b-input>
            </div>
          </b-field>

          <b-field label="Select Extra/ Deduction/ Paid-Offs">
            <div class="is-flex is-flex-direction-column">
              <div v-for ="(item, index) in recordTypes" :key="index">
                <b-radio v-model="model.recordType" 
                  :native-value="item.id"
                  type="is-info">
                  {{item.name}}
                </b-radio>
              </div> 
            </div>
            
          </b-field>

          <b-field label="Reason">
            <b-input
              type="textarea"
              v-model="model.reason"
              required maxlength="500">
            </b-input>
          </b-field>
          <b-field label="Start date">             
            <b-datepicker
              icon="calendar-today"
              locale="en-SG"
              v-model="startDate"
              editable required>
            </b-datepicker>
          </b-field>

          <b-field label="End date">
            <b-datepicker
              icon="calendar-today"
              locale="en-SG"
              v-model="endDate"
              editable required>
            </b-datepicker>
          </b-field>

          <b-field label="Remarks">
            <b-input
              type="text"
              v-model="model.remarks"
              required maxlength="200">
            </b-input>
          </b-field> 
          <b-field class="file is-primary" :class="{ 'has-name': !!files }">
            

          <div class="is-flex is-flex-direction-column">
            <p class="is-size-6 has-text-weight-bold">MCs or other related pics</p>
            
            <div class="my-3">
              <b-upload :loading="isLoadingFiles" multiple v-model="files" class="file-label" @change.native="uploadFiles" 
              accept=".xlsx, .xls, .csv, .doc, .docx, .pdf, .png, .jpg, .jpeg, .bmp, .TIFF, .HEIC" required validationMessage="Please select correct file type">
              <span class="file-cta">
                <b-icon class="file-icon" icon="upload" ></b-icon>
                <span class="file-label" >Add files</span>
              </span>          
              </b-upload>
            </div>

            <div v-if="model.staffRecordDocuments.length>0">
              <b-tag v-for="(fileName, index) in model.staffRecordDocuments "
              :key="index"
              type="is-info"
              class="is-flex my-3" >
              <span class="mr-3">{{fileName}}</span>
              <button class="delete is-small"
                type="button"
                @click="files.splice(index, 1)">
              </button>
              </b-tag>    
            </div> 

            <div v-else>
              <b-tag v-for="(fileItem, index) in files"
              :key="index"
              type="is-info"
              class="is-flex my-3" >
              <span class="mr-3">{{fileItem.name}}</span>
              <button class="delete is-small"
                type="button"
                @click="files.splice(index, 1)">
              </button>
              </b-tag> 
            </div>

           
          </div>

         </b-field>
        </section>
        <footer class="modal-card-foot">
          <b-button label="Close" @click="cancelCreateOrUpdate" />
          <b-button :label="model.id==0?'Create':'Update'" type="is-primary" @click="createOrUpdateModel"/>
        </footer>
      </div>
    </b-modal>

    <b-modal v-model="isDeleteModalActive" trap-focus has-modal-card auto-focus :can-cancel="false" scroll="keep">
      <div class="modal-card" style="width:300px">
        <header class="modal-card-head">
          <p class="modal-card-title">Are you sure to delete this data</p>                 
        </header>
        <footer class="modal-card-foot">
          <b-button label="Cancel" @click="isDeleteModalActive=false; selectedId=null" />
          <b-button label="Confirm" type="is-info" @click="deleteData"/>
        </footer>
        </div>
    </b-modal>
  </section>
</template>
<script>
import moment from "moment";
import { getDetail, getList, createOrUpdate, deleteData , getEmployeeByBrand } from "@/api/staffRecord";
import { getDropdown as getDepartmentDropdown } from "@/api/department";
import { uploadFiles  } from "@/api/fileService";
export default {
  name:"staffRecord",
  created() {
    this.getEmployeeByBrand();
    this.getDepartmentDropdown();
    this.getList();
  },
  data() {
    return {
      data: [],
      files:[],
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
      employees:[],
      filterEmployees:[],
      departments:[],
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
        status:true,
        id:0,
        employeeId:0,
        departmentId:0,
        otherDepartment:null,
        reason:'',
        remarks:null,
        staffRecordDocuments:[]
      },
      defaultModel:{
        status:true,
        id:0,
        employeeId:0,
        departmentId:0,
        otherDepartment:null,
        reason:'',
        remarks:null,
        staffRecordDocuments:[]
      },
      isModalActive:true,
      isDeleteModalActive:false,
      selectedId:null,
      selected:null,
      searchEmployee: '',
      startDate:null,
      endDate:null,
      isLoadingFiles:false,
      recordTypes:
      [
        {id:0,name:'Extra pay (OTs, Cover Shift)'},
        {id:1,name:'Deduction (Late, Unpaid leave)'},
        {id:2,name:'Paid-Offs'},
        {id:3,name:'Paid-MCs'},
      ]
    };
  },
  watch: {},
  computed: {
    canCreate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "StaffRecord.Create"
        )
      );
    },
    canUpdate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "StaffRecord.Update"
        )
      );
    },
    canDelete() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "StaffRecord.Delete"
        )
      );
    },
    filteredDataArray() {
      if(!this.searchEmployee) return this.employees;
      return this.employees.filter((option) => {
          return option
              .toString()
              .toLowerCase()
              .indexOf(this.searchEmployee.toLowerCase()) >= 0
      })
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
          this.openErrorMessage(error.response.status); 
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    closeModalDialog(){
      this.model= {...this.defaultModel};
      this.isModalActive= false;
      this.startDate = null;
      this.endDate = null;
    },
    cancelCreateOrUpdate(){
      this.closeModalDialog();
    },
    createOrUpdateModel(){
      if(this.startDate)
        this.model.startDate = `${this.startDate.getFullYear()}-${('0' + (this.startDate.getMonth()+1)).slice(-2)}-${('0' + this.startDate.getDate()).slice(-2)}`;

      if(this.endDate)
        this.model.endDate = `${this.endDate.getFullYear()}-${('0' + (this.endDate.getMonth()+1)).slice(-2)}-${('0' + this.endDate.getDate()).slice(-2)}`;
      
      createOrUpdate(this.model)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `${this.model.id==0?'Create':'Update'} successfully!`,
              queue: false,
            });
          }
        })
      .catch((error) => {
          this.openErrorMessage(error.response.status); 
        })
      .finally(() => {
        this.closeModalDialog();
        this.getList();
      });
    },
    getAsyncData(){
      let searchEmployee= this.searchEmployee.toLowerCase();         
      if(!searchEmployee) this.filterEmployees=[...this.employees];
          let array= this.employees.filter((option) => {
            console.log(option);
          return (option.employeeCode
                      .toLowerCase()
                      .indexOf(searchEmployee) >= 0)
                ||(option.name
                      .toLowerCase()
                      .indexOf(searchEmployee) >= 0)
      });
      console.log(array.length);
      console.log(this.employees.length);
      this.filterEmployees=[...array];
      },
    editModel(input){
      this.model= {...input};
      this.startDate= moment(this.model.startDate,'YYYY-MM-DD').toDate();
      this.endDate= moment(this.model.endDate,'YYYY-MM-DD').toDate();
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
      .catch((error) => {
          this.openErrorMessage(error.response.status); 
        })
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
    getEmployeeByBrand(){
      getEmployeeByBrand()
      .then((response) => {
        if (response.status == 200) {
          this.employees= [... response.data]; 
          this.filterEmployees= [... response.data]; 
          }
        })
      .catch((error) => {
          this.openErrorMessage(error.response.status); 
        })
      .finally(() => {});
    },
    getDepartmentDropdown() {
      getDepartmentDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.departments = [...response.data];
            this.departments.push({id:0,name:'Other: '})
          } 
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
        });
    },
    getDetail(id){
      if(!id) return;
      getDetail(id)
      .then((response) => {
        if (response.status == 200 && response.data) {
          this.model= {...response.data};
          this.startDate= moment(this.model.startDate,'YYYY-MM-DD').toDate();
          this.endDate= moment(this.model.endDate,'YYYY-MM-DD').toDate();
          const index = this.employees.findIndex(item => item.id === this.model.employeeId);
          //console.log(index);
          this.selected=index>=0? this.employees[index]:null;
          this.searchEmployee=index>=0? this.employees[index].fullName:'';
          this.isModalActive= true;
          }
        })
      .catch((error) => {this.notifyErrorMessage(error)})
      .finally(() => {});
    },
    uploadFiles(){
      console.log('uploadFiles');
      this.isLoadingFiles = true;
      let formData = new FormData();
      for (let i = 0 ; i < this.files.length ; i++) {
        formData.append('files', this.files[i],this.files[i].name);
      }
      uploadFiles({folderName:'StaffRecord'},formData)
        .then((response) => {
          if (response.status == 200) {
            this.model.staffRecordDocuments =[...response.data];
        }})
        .catch((error) => {
          // this.$buefy.snackbar.open({
          //   message: error,
          //   queue: false,
          //   type: 'is-warning'
          // });
        })
        .finally(() => {
          this.isLoadingFiles = false;
        });
    }
  }
};
</script>
