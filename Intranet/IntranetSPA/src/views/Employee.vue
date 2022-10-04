<template>
  <section class="section is-main-section">
    <div class="p-2">
      <b-button
          v-if="canCreate"
          label="Import"
          type="is-primary"
          class="mr-4"
          icon-left="note-plus"
          :size="$isMobile()?'is-small':''"
          @click="isModalImportActive=true"
          :loading="isImportLoading"          
        />
      <b-button
        label="Create"
        type="is-info"
        class="mr-4"
        :size="$isMobile()?'is-small':''"
        icon-left="note-plus"
        @click="isModalActive=true"
        v-if="canCreate"
      />
    </div>
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
    narrowed
    hoverable
    class="customTable"
    >
      <b-table-column
        field="Name"
        label="Name"
        sortable        
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"        
        v-slot="props"
      >       
      {{props.row.name}}
      </b-table-column>

      <b-table-column
        field="employeeCode"
        label="Employee code"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        sortable        
        width="250"
        v-slot="props"
      >       
      {{props.row.employeeCode}}
      </b-table-column>

      <b-table-column
        field="rankId"
        label="Rank"
        width="150"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"      
        v-slot="props"
      >       
      {{props.row.rank}}
      </b-table-column>
       
      <b-table-column
        field="dept"
        label="Department"
        width="150"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"        
        v-slot="props"
      >       
      {{props.row.dept}}
      </b-table-column>

       <b-table-column
        field="dept"
        label="Brand"
        width="150"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"        
        v-slot="props"
      >       
        <p v-for="(brandName,index) in props.row.brands" :key="index">{{brandName}}</p> 
      </b-table-column>

      <b-table-column
        field="Country"
        label="Country"
        width="150"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"          
        v-slot="props">       
        {{props.row.country}}
      </b-table-column>

      <b-table-column
        field="salary"
        label="Salary"
        width="150" 
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"         
        v-slot="props">       
        {{props.row.currencySymbol}} {{props.row.salary|formattedNumber}}
      </b-table-column>

      <b-table-column
        field="Status"
        label="Status"
        width="100"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'" 
        sortable
        v-slot="props">        
       <span :class="props.row.status?'':'has-text-danger'">{{ props.row.status?'Active':'Inactive' }}</span>        
      </b-table-column>

      <b-table-column
        field="startDate"
        label="Start Date"
        sortable
        v-slot="props"
        width="150"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
        {{ props.row.startDate | dateTime('DD-MM-YYYY') }}       
      </b-table-column>

      <b-table-column
        field="CreationTime"
        label="Creation Time"
        sortable
        v-slot="props"
        width="150"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
        {{ props.row.creationTime | dateTime }}        
      </b-table-column>

      <b-table-column
        field="LastModificationTime"
        label="Last Update Time"
        v-slot="props"
        width="200"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
        {{ props.row.lastModificationTime | dateTime }}
      </b-table-column>

      <b-table-column
        field="lastModifierUser"
        label="Last Update By"
        v-slot="props"
        width="200"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'">
        {{ props.row.lastModifierUser }}
      </b-table-column>

      <b-table-column
        field="Edit"
        label="Edit"        
        v-slot="props"
        width="120"
        centered
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        >
        <div class="is-flex is-flex-direction-row is-justify-content-space-between">
          <a 
            v-if="canUpdate"
            title="edit"
            @click="editModel(props.row)">
              <b-icon icon="pencil" type="is-info" ></b-icon>
          </a>
          <a 
            v-else
            title="view"
            @click="editModel(props.row)">
              <b-icon icon="eye" :size="'is-small'" style="color:#4a4a4a" ></b-icon>
          </a> 

        <a 
          v-if="canDelete"
          title="delete"          
          class="ml-3 has-text-grey"
          @click="deleteSelectedModel(props.row.id)">
            <b-icon icon="delete"></b-icon>
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
          v-if="canCreate"
          label="Import"
          type="is-primary"
          class="mr-4"
          icon-left="note-plus"
          :size="$isMobile()?'is-small':''"
          @click="isModalImportActive=true"
          :loading="isImportLoading"          
        />
        <b-button
          v-if="canCreate"
          label="Create"
          type="is-info"
          class="mr-4"
          icon-left="note-plus"
          :size="$isMobile()?'is-small':''"
          @click="isModalActive=true"
        />
        <b-button
            label="Reset"
            type="is-light"
            class="mr-4"
            icon-left="reload"
            :size="$isMobile()?'is-small':''"
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
        <!-- <form :method="model.id==0?'POST':'PUT'"> -->
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
                  v-model.trim="model.name"
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
                  v-model="model.idNumber">
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
                <multiselect
                  ref="multiselectRank"
                  v-model="selectRank"
                  tag-placeholder=""
                  placeholder="Select rank"             
                  :options="ranks"
                  label="name"
                  track-by="id"
                  :multiple="false"
                  :taggable="false"
                  :close-on-select="true"
                  :clear-on-select="true"
                  selectLabel=""
                  deselectLabel="Remove"
                  @select="(selectedOption, id)=>{ model.rankId=selectedOption.id }"
                  @remove="(removedOption, id)=>{ model.rankId=null }"
                  >
                  <span  slot="noResult">No result found</span>
                </multiselect>
                  
              </b-field>

              <b-field label="Bank" class="column is-3">
                <multiselect
                  ref="multiselectBank"
                  v-model="selectBank"
                  tag-placeholder=""
                  placeholder="Select bank"             
                  :options="banks"
                  label="name"
                  track-by="id"
                  :multiple="false"
                  :taggable="false"
                  :close-on-select="true"
                  :clear-on-select="true"
                  selectLabel=""
                  deselectLabel="Remove"
                  @select="(selectedOption, id)=>{ model.bankId=selectedOption.id }"
                  @remove="(removedOption, id)=>{ model.bankId=null }"
                  >
                  <span  slot="noResult">No result found</span>
                </multiselect>          
              </b-field>

              <b-field label="Bank Account Name" class="column is-3">
                <b-input
                  type="Text"
                  v-model="model.bankAccountName">
                </b-input>
              </b-field>

              <b-field label="Bank Account Number" class="column is-3">
                <b-input
                  type="Text"
                  v-model="model.bankAccountNumber">
                </b-input>
              </b-field>
            </div>        

            <div class="columns">
              <b-field label="Department" class="column is-3">
                <multiselect
                  ref="multiselectDepartment"
                  v-model="selectDepartment"
                  tag-placeholder=""
                  placeholder="Select department"             
                  :options="departments"
                  label="name"
                  track-by="id"
                  :multiple="false"
                  :taggable="false"
                  :close-on-select="true"
                  :clear-on-select="true"
                  selectLabel=""
                  deselectLabel="Remove"
                  @select="(selectedOption, id)=>{ model.deptId=selectedOption.id }"
                  @remove="(removedOption, id)=>{ model.deptId=null }"
                  >
                  <span  slot="noResult">No result found</span>
                </multiselect>
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
                  v-model="model.backendUser">
                </b-input>
              </b-field>

              <b-field label="Backend Pass" class="column is-3">
                <b-input
                  type="Text"
                  v-model="model.backendPass">
                </b-input>
              </b-field>
            </div>

            <div class="columns">
              <b-field label="Brand" class="column is-3">
                <multiselect
                ref="multiselectBrand"
                v-model="selectBrands"
                tag-placeholder=""
                placeholder="Select brand"             
                :options="brands"
                label="name"
                track-by="id"
                :multiple="true"
                :taggable="false"
                selectLabel="Add"
                deselectLabel="Remove"
                @select="(selectedOption, id)=>{model.brand=selectedOption;model.brandId= id  }"
                >
                <span  slot="noResult">No result found</span>
                </multiselect>
              </b-field>
              <b-field label="Intranet Role" class="column is-3">
                <multiselect
                  v-model="selectRole"
                  tag-placeholder=""
                  placeholder="Select role"             
                  :options="roles"
                  label="name"
                  track-by="id"
                  :multiple="false"
                  :taggable="false"
                  :close-on-select="true"
                  :clear-on-select="true"
                  selectLabel=""
                  deselectLabel="Remove"
                  @select="(selectedOption, id)=>{ model.roleId=selectedOption.id }"
                  @remove="(removedOption, id)=>{ model.roleId=null }"
                  >
                  <span  slot="noResult">No result found</span>
                </multiselect>
              </b-field>

              <b-field label="Intranet User" class="column is-3">
                <b-input
                  type="Text"
                  v-model="model.employeeCode"
                  required>
                </b-input>
              </b-field>
  
              <b-field class="column is-3" :type="validPasswordMessages.length>0? 'is-danger':''" :message="validPasswordMessages">
                <template #label>
                  Intranet Pass
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
                  v-model.trim="model.intranetPassword"
                  type="text"
                  name="password"
                  required
                  minlength="8"
                  maxlength="30"
                  icon-right="refresh"
                  icon-right-clickable
                  @icon-right-click="model.intranetPassword=generateRandomPassword()"
                />
              </b-field>
            </div>   

            <div class="columns">
              <b-field label="Salary" class="column is-3">
                <b-input
                  type="number"
                  v-model="model.salary"
                  min="0"
                  required>
                </b-input>
              </b-field>

              <b-field label="Country" class="column is-3">
                <b-input
                  type="Text"
                  v-model="model.country">
                </b-input>
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
            <b-button v-if="(canCreate && model.id==0)||(canUpdate && model.id>0)" native-type="submit" :label="model.id==0?'Create':'Update'" type="is-primary" @click="createOrUpdateModel"/>
          </footer>
        <!-- </form>        -->
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
          <b-button label="Close" @click="file=null;isShowImportResult=false;isModalImportActive=false" />
          <b-button label="Import Data" type="is-primary" :disabled="file==null" @click="importEmployees" :loading="isImportLoading"/>
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
import _ from 'lodash';
import moment from "moment";
import Multiselect from "vue-multiselect";
// import { ValidationObserver, ValidationProvider } from "vee-validate";
import { getDetail, getList, createOrUpdate, deleteData, importEmployees, getRelatedData  } from "@/api/employee";
import { getDropdown as getRoleDropdown } from "@/api/role";
import { getDropdown as getBankDropdown } from "@/api/bank";
import { getDropdown as getBrandDropdown } from "@/api/brand";
import { getDropdown as getDepartmentDropdown } from "@/api/department";
import { getDropdown as getRankDropdown } from "@/api/rank";
export default {
  name:"employee",
  components: { 
    Multiselect, 
    // ValidationObserver, 
    // ValidationProvider 
  },
  created() {
    this.getList();
    this.getRelatedData(); 
  },
  data() {
    return {
      file: null,
      fileName:'',
      isShowImportResult:false,
      isImportLoading:false,
      errorList:[],
      validPasswordMessages:[],
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
        id:0,
        intranetUsername:'',
        intranetPassword:'',
        salary:0
      },
      defaultModel:{
        name:null,
        status:true,
        id:0,
        intranetUsername:'',
        intranetPassword:'',
        salary:0
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
      selectBrands:[],
      selectRank:null,
      selectBank:null,
      selectDepartment:null,
      selectRole:null,
    };
  },
  watch: {
    "model.intranetPassword":_.debounce(function(value){
      this.validPasswordMessages=value? this.checkValidPassword(value):[]; 
    },800)
  },
  computed: {
    canCreate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Employee.Create"
        )
      );
    },
    canUpdate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Employee.Update"
        )
      );
    },
    canDelete() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "Employee.Delete"
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
      this.birthDate = null;
      this.startDate = null;
      this.selectBrands= [];
      this.selectRank=null;
      this.selectBank=null;
      this.selectDepartment=null;
      this.selectRole=null;
      this.isModalActive= false;
    },
    cancelCreateOrUpdate(){
      this.closeModalDialog();
    },
    createOrUpdateModel(){
      if(!this.model.name){
        this.$buefy.snackbar.open({
          message: 'Please check field Name',
          queue: false,
          duration: 2000,
          type: 'is-warning'
        });
        return;
      }

      if(!this.model.employeeCode){
        this.$buefy.snackbar.open({
          message: 'Please check field Employee Code',
          queue: false,
          duration: 2000,
          type: 'is-warning'
        });
        return;
      }

      this.validPasswordMessages= this.checkValidPassword(this.model.intranetPassword);
      if(this.validPasswordMessages.length>0){
        this.$buefy.snackbar.open({
          message: 'Please check field Intranet Password!',
          queue: false,
          duration: 1000,
          type: 'is-danger'
        });
        return;
      }

      if(!this.model.roleId){
        this.$buefy.snackbar.open({
          message: 'Please check field Intranet Role',
          queue: false,
          duration: 2000,
          type: 'is-warning'
        });
        return;
      } 

    this.model.birthDate =this.convertDateToString(this.birthDate);
    this.model.startDate =this.convertDateToString(this.startDate);    
    this.model.brandIds = this.selectBrands.length>0? this.selectBrands.map((p) => p.id):[]; 
           
    createOrUpdate(this.model)
      .then((response) => {
        if (response.status == 200) {
          //this.$router.replace({'query': null});
          this.$buefy.snackbar.open({
              message: `${this.model.id==0?'Create':'Update'} successfully!`,
              queue: false,
            });
          }
          this.closeModalDialog();
          this.getList();
        })
      .catch((error) => {this.notifyErrorMessage(error)})
      .finally(() => { });
    },
    editModel(input){
      this.model= {...input};
      if(this.model.birthDate)
        this.birthDate= moment(this.model.birthDate,'YYYY-MM-DD').toDate();

      if(this.model.startDate)
        this.startDate= moment(this.model.startDate,'YYYY-MM-DD').toDate();

      if(this.model.brandIds)
        this.selectBrands= this.brands.filter(p=> this.model.brandIds.includes(p.id));
      
      if(this.model.rankId)
        this.selectRank= this.ranks.find(item=> item.id== this.model.rankId);

      if(this.model.bankId)
        this.selectBank= this.banks.find(item=> item.id== this.model.bankId);

      if(this.model.deptId)
        this.selectDepartment= this.departments.find(item=> item.id == this.model.deptId);
 
      if(this.model.roleId)
        this.selectRole= this.roles.find(item=> item.id== this.model.roleId);
      
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
          this.notifyErrorMessage(error)
        })
        .finally(() => {
          this.isImportLoading = false;
          this.file=null;
        });
    },
    downloadErrorListExcel() {
      if (this.errorList.length > 0) 
         this.exportExcelData(this.errorList, "ErrorList", 30);
    },
    getRelatedData() {
      getRelatedData()
        .then((response) => {
          if (response.status == 200) {
            let data= response.data;
            this.roles = data.roles;
            this.departments = data.departments;
            this.banks = data.banks;
            this.brands = data.brands;
            this.ranks = data.ranks;
          } 
        })
        .catch((error) => {this.notifyErrorMessage(error)})
        .finally(() => {});
    },
    getRoleDropdown() {
      getRoleDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.roles = [...response.data];
          } 
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
        });
    },
    getDepartmentDropdown() {
      getDepartmentDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.departments = [...response.data];
          } 
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
        });
    },
    getBankDropdown() {
      getBankDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.banks = [...response.data];
          } 
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
        });
    },
    getBrandDropdown() {
      getBrandDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.brands = [...response.data];
          } 
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
        });
    },
    getRankDropdown() {
      getRankDropdown()
        .then((response) => {
          if (response.status == 200) {
            this.ranks = [...response.data];
          } 
        })
        .catch((error) => {
          this.notifyErrorMessage(error)
        })
        .finally(() => {
        });
    },
    generatePassword(length = 8) {
      let  charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      let  retVal = "";
      for (var i = 0, n = charset.length; i < length; ++i) {
          retVal += charset.charAt(Math.floor(Math.random() * n));
      }
      return retVal;
    }
  }
};
</script>
