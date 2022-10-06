<template>
  <section class="section is-main-section">
    <b-table
      :data="data"
      :loading="isLoading"      
      backend-pagination
      backend-sorting
      :default-sort="filter.sortBy"
      :default-sort-direction="filter.sortDirection"
      :sort-icon="sortIcon"
      :sort-icon-size="sortIconSize"
      @sort="onSort"
      :debounce-page-input="200"
      mobile-cards
    >
      <b-table-column
        field="Name"
        label="Role"
        sortable        
        v-slot="props"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
      >       
      {{ props.row.name}}
      </b-table-column>

      <b-table-column
        field="Count"
        label="Count"
        width="120"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
        <a v-if="props.row.employees &&props.row.employees.length>0" 
            @click="isEmpoyeeListModalActive=true;selectedRoleName=props.row.name;employeeList=[...props.row.employees]"> 
            {{ props.row.employees.length}}
        </a>
        <span v-else>0</span>
      </b-table-column>

      <b-table-column
        field="CreationTime"
        label="Creation Time"
        sortable
        v-slot="props"
        width="200"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
       {{ props.row.creationTime | dateTime('DD-MM-YYYY hh:mm:ss') }} 
      </b-table-column>

      <b-table-column
        field="CreatorUserId"
        label="Created By"
        width="200"
        sortable
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.creatorUser}}
      </b-table-column>

      <b-table-column
        field="LastModificationTime"
        label="Updated On"
        sortable
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
        width="200">
       {{ props.row.lastModificationTime | dateTime('DD-MM-YYYY hh:mm:ss') }} 
      </b-table-column>

      <b-table-column
        label="Updated By"
        field="LastModifierUserId"
        width="200"
        sortable
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.lastModifierUser}}
      </b-table-column>

      <b-table-column
        field="Status"
        label="Status"
        sortable
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
        width="100">        
       <span :class="props.row.status?'':'has-text-danger'">{{ props.row.status?'Active':'Disabled' }}</span>        
      </b-table-column>

      <b-table-column
        field="Edit"
        label="Edit"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"       
        v-slot="props"
        centered
        width="100"> 
        <div class="is-flex is-flex-direction-row is-justify-content-space-between">    
          <a 
            v-if="canUpdate"
            title="edit"          
            @click="getDetail(props.row.id)">
            <b-icon
              icon="pencil"
              type="is-info">
            </b-icon>
          </a> 
          
          <a 
            v-if="canDelete"
            title="delete"          
            class="ml-3 has-text-grey"
            @click="deleteSelectedModel(props.row.id)">
            <b-icon
              icon="delete">
            </b-icon>
          </a>       
        </div>   
      </b-table-column>

      <template #empty>
        <div class="has-text-centered">No records</div>
      </template>
      
      <div slot="footer" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">
        <b-button
            label="Create"
            type="is-info"
            class="mr-4"
            :size="$isMobile()?'is-small':''"
            icon-left="note-plus"
            @click="isModalActive=true"
            v-if="canCreate"
          />
        <b-button
            label="Reset"
            type="is-light"
            class="mr-4"
            :size="$isMobile()?'is-small':''"
            icon-left="reload"
            @click="resetFilter"
        />
        <span :class="$isMobile()?'is-size-7 has-text-weight-normal mr-4': 'has-text-weight-normal mr-4'">Total count: {{totalItems}}</span>
        <b-select :size="$isMobile()?'is-small':''" v-model="filter.rowsPerPage"  @input="onChangePageSize" class="mr-4">
            <option v-for="i in pageOptions" :value="i" :key="i">{{`${i} per page`}}</option>        
        </b-select>
        <b-pagination
            :total="totalItems"
            v-model="filter.page"
            :range-before="1"
            :range-after="1"
            :order="`is-right`"
            :size="$isMobile()?'is-small':''"
            :simple="false"
            :rounded="false"
            :per-page="filter.rowsPerPage"
            :icon-prev="`chevron-left`"
            :icon-next="`chevron-right`"
            aria-next-label="Next page"
            aria-previous-label="Previous page"
            aria-page-label="Page"
            aria-current-label="Current page"
            :page-input="!$isMobile()"
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
          <b-field label="Name">
              <b-input
                type="Text"
                v-model.trim="model.name"
                placeholder="Name...."
                required>
              </b-input>
          </b-field>

          <b-field>
            <template #label>
              <span class="mr-3">Departments</span> 
              <b-button label="Select all" type="is-info" size="is-small" @click="selectAllDepartments"/>
            </template>
            <div class="is-flex is-flex-wrap-wrap is-justify-content-flex-start is-align-items-stretch">
              <b-checkbox v-for="(item,index) in departments" :key="index" :native-value="item.id" v-model="model.departmentIds" class="mr-3 p-3"
                type="is-info">{{item.name}}
              </b-checkbox>
            </div>                
          </b-field>
            
          <b-field>
            <template #label>
              <span class="mr-3">Permissions</span> 
              <b-button label="Select all" type="is-info" size="is-small" @click="selectAllPermissions"/>
            </template>
            <treeselect 
              v-model="model.permissions" 
              :multiple="true" 
              :options="permissions"  
              :disable-branch-nodes="true" 
              search-nested 
              always-open/>
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
          <p class="modal-card-title">Are you sure to delete this data?</p>                 
        </header>
        <footer class="modal-card-foot">
          <b-button label="Cancel" @click="isDeleteModalActive=false; selectedId=null" />
          <b-button label="Confirm" type="is-info" @click="deleteData"/>
        </footer>
        </div>
    </b-modal>

    <b-modal v-model="isEmpoyeeListModalActive" has-modal-card scroll="keep">
      <div class="modal-card">
        <header class="modal-card-head">
          <p class="modal-card-title">{{selectedRoleName}}</p>                 
        </header>
        <section class="modal-card-body">
          <b-table :data="employeeList" sticky-header style="max-height: 50vh;overflow-y: auto;">
            <b-table-column field="EmployeeCode" label="Employee Code" width="250" v-slot="props"
                header-class="is-size-7 customTableBorderHeader"
                :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
                {{ props.row.employeeCode }}
            </b-table-column>
            <b-table-column field="name" label="Name" width="350" v-slot="props"
                header-class="is-size-7 customTableBorderHeader"
                :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
                {{ props.row.name }}
            </b-table-column>            
          </b-table>
        </section>
        <footer class="modal-card-foot">
          <b-button label="Close" @click="isEmpoyeeListModalActive=false" />
        </footer>
        </div>
    </b-modal>
  </section>
</template>
<script>
import { getDetail, getList, createOrUpdate, deleteData, getAllPermissions  } from "@/api/role";
import { getSimpleList as getDepartments } from "@/api/department";
// import permissions from '@/utils/permissions.js'
// import the component
import Treeselect from '@riophae/vue-treeselect'
// import the styles
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
export default {
  name:"role",
  components: { Treeselect },
  created() {
    this.getList();
    this.getAllPermissions();
    this.getDepartments();
  },
  data() {
    return {
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
      sources:[],
      searchSource:null,
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
        permissions:[],
        departmentIds:[],
        id:0
      },
      defaultModel:{
        name:null,
        status:true,
        permissions:[],
        departmentIds:[],
        id:0
      },
      isModalActive:false,
      isDeleteModalActive:false,
      isEmpoyeeListModalActive: false,
      selectedId:null,
      selectedRoleName:null,
      employeeList:[],
      departments:[],
      selectDepartments:[],      
      permissions:[]
    };
  },
  watch: {},
  computed: {
    canCreate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Role.Create"
        )
      );
    },
    canUpdate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Role.Update"
        )
      );
    },
    canDelete() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Role.Delete"
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
          this.notifyErrorMessage(error)
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    closeModalDialog(){
      this.model= {...this.defaultModel};
      this.isModalActive= false;
    },
    cancelCreateOrUpdate(){
      this.closeModalDialog();
    },
    createOrUpdateModel(){
      createOrUpdate(this.model)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `${this.model.id==0?'Create':'Update'} successfully!`,
              queue: false,
            });
          this.closeModalDialog();
          this.getList();
          }
        })
      .catch((error) => {this.notifyErrorMessage(error)})
      .finally(() => {});
    },
    editModel(input){
      this.model= {...input};
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
      .catch((error) => {this.notifyErrorMessage(error)})
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
    getDetail(id){
      if(!id) return;
      getDetail(id)
      .then((response) => {
        if (response.status == 200 && response.data) {
          this.model= {...response.data};          
          this.isModalActive= true;
          }
        })
      .catch((error) => {this.notifyErrorMessage(error)})
      .finally(() => {});
    },
    selectAllPermissions(){
      this.model.permissions= this.permissions.flatMap(p=>p.children).map(p=>p.id);
    },
    selectAllDepartments(){
      this.model.departmentIds= this.departments.map(p=>p.id);
    },
    getDepartments() {
      getDepartments()
        .then((response) => {
          if (response.status == 200) {
            this.departments = response.data;
          } 
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
        });
    },
    getAllPermissions() {
      getAllPermissions()
        .then((response) => {
          if (response.status == 200) {
            this.permissions = response.data;            
          }
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
  }
};
</script>
