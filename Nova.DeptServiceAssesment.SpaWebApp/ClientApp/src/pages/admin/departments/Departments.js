import React, { Component } from 'react';
import { Spinner, Button, Card, CardBody, CardHeader, Col, Row, Table,Modal, ModalHeader, ModalBody, Form, FormGroup, 
        Label, Input, InputGroup, InputGroupAddon  } from 'reactstrap';
import { DataService } from '../../../services';
import { CustomPagination } from '../../../components';
import ReactHTMLTableToExcel from 'react-html-table-to-excel';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';

class Departments extends Component {
  constructor(props) {
    super(props);
    this.state = {
      departments: [],
      saving: false,
      importing: false,
      isUpdloadOpen: false,
      file: null,
      searchTerm: '',
      searching: false,
      activePage: 1,
      itemsPerPage: 25
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  queryData = (searchterm, activePage, itemsPerPage) => {
    DataService.getDepartments(searchterm, activePage, itemsPerPage)
      .then(result => {
        this.setState({        
          departments: result.data.data,
          searchTerm: searchterm,  
          activePage : activePage,
          itemsPerPage: itemsPerPage,
          totalRecords: result.data.totalRecords,
          searching: false
        });
      })
      .catch(error => {
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  componentDidMount = () => {
    this.queryData('', 1, 25);
  }

  handlePaging = (number) => {
    
    if(number != this.state.activePage){
      this.setState({ departments: null });
      this.queryData(this.state.searchTerm, number, this.state.itemsPerPage);
    }

  }

  onFormSubmit = (e) => {
    e.preventDefault() // Stop form submit
    this.setState({importing : true})
    DataService.importDepartments(this.state.file)
        .then(result => {
          this.setState({          
            file: null,
          });
  
          this.queryData(this.state.searchTerm, 1, this.state.itemsPerPage);
          this.setState({importing : false, isUpdloadOpen: false});
          this.props.toggleModal(true, 'Done','Upload thành công');
        })
        .catch(error => {
          this.setState({importing : false, isUpdloadOpen: false});
          this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
        });
  }
  
  onChange = (e) => {
    this.setState({file:e.target.files[0]})
  }

  syncDepartments = (e) => {
    e.preventDefault();
    this.setState({saving: true});

    DataService.syncDepartments()
        .then(result => {
            this.setState({saving: false});
            this.props.toggleModal(true, 'Done','Đồng bộ dữ liệu thành công');
        })
        .catch(error => {
            this.setState({saving : false});
            let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
            if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
            msg = error.response.data.errors.domainValidations[0];
            this.props.toggleModal(true, 'Lỗi', msg);
        });
  }

  openModal = () => {
    this.setState({isUpdloadOpen: true});
  }
  
  closeModal = () => {
      this.setState({isUpdloadOpen: false});
  }

  handleChange = (event) => {

    const {id, name, value, type} = event.target;

    this.setState({ [name]: value });

    if(name === 'itemsPerPage'){
      this.setState({ departments: null });
      this.queryData(this.state.searchTerm, 1, value);
    }

    
  }

  handleSearch = (event) => {
    
    this.setState({ searching: true, departments: null });
    this.queryData(this.state.searchTerm, 1, this.state.itemsPerPage);

  }

  render() {

    let dataRows =  this.state.departments == null ?
                    <tr>
                      <td colSpan="5" align="center">Loading</td>
                    </tr> :
                    this.state.departments.length === 0 ?
                    <tr>
                      <td colSpan="5" align="center">Không có dữ liệu</td>
                    </tr> :
                    this.state.departments.map((p, i) => {
                      return (
                          <tr key={i}>
                            <td>{(this.state.activePage-1)*this.state.itemsPerPage+i+1}</td>
                            {/* <td>{p.divCode}</td> */}
                            <td>{p.divName}</td>
                            {/* <td>{p.shortCode}</td> */}
                            <td>{p.name}</td>
                            <td>{p.emailHead}</td>
                          </tr>
                      )
                    });

    const syncbtn = this.state.saving ?
                    <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                    <Button type="button" size="sm" color="success" className="float-right" onClick={e => this.syncDepartments(e)}><i className="fa fa-refresh"></i> Đồng bộ</Button>;
    
    const uploadbtn = this.state.importing ?
                    <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                    <Button type="submit" color="secondary">Submit</Button>;

    const searchbtn = this.state.searching ?
                    <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /></Button> :
                    <Button type="button" color="primary"  onClick={e => this.handleSearch(e)}><i className="fa fa-search"></i></Button>;

    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Danh sách phòng ban
                <div className="card-header-actions">
                   {syncbtn}<div className="float-right">&nbsp;</div>
                   <Button type="button" size="sm" color="success" className="float-right" onClick={this.openModal}><i className="fa fa-file"></i> Import</Button>
                   <div className="float-right">&nbsp;</div>
                   <ReactHTMLTableToExcel
                                id="table-xls-button"
                                className="btn btn-primary btn-sm float-right"
                                table="tableDepartments"
                                filename="departments"
                                sheet="tablexls"
                                buttonText="Download as XLS"/>
                </div>
              </CardHeader>
              <CardBody>
                <FormGroup row>
                    <Col xs="9" md="9">
                      <InputGroup>
                        <Input type="text" id="searchTerm" name="searchTerm" placeholder="Tên phòng ban" 
                                            value={this.state.searchTerm} onChange={this.handleChange}/>
                        <InputGroupAddon addonType="prepend">
                          {searchbtn}
                        </InputGroupAddon>
                      </InputGroup>
                    </Col>
                    <Col xs="3" md="3">
                      <Input  type="select" name="itemsPerPage" id="itemsPerPage"
                                value={this.state.itemsPerPage}
                                onChange={this.handleChange}>
                            <option value='25'>25</option>
                            <option value='50'>50</option>
                            <option value='100'>100</option>
                            <option value='150'>150</option>
                            <option value='200'>200</option>
                            <option value='250'>250</option>
                            <option value='300'>300</option>
                            <option value='350'>350</option>
                            <option value='400'>400</option>
                            <option value='450'>450</option>
                            <option value='500'>500</option>
                        </Input>
                    </Col>
                </FormGroup>
                <Table hover bordered striped responsive id="tableDepartments">
                  <thead>
                  <tr>
                    <th width="80">STT</th>
                    {/* <th width="120">Mã Khối</th> */}
                    <th>Khối</th>
                    {/* <th width="120">Mã phòng</th> */}
                    <th>Phòng</th>
                    <th width="200">Email Head</th>
                  </tr>
                  </thead>
                  <tbody>
                    {dataRows}
                  </tbody>
                </Table>
                <CustomPagination activePage={this.state.activePage} 
                            itemsPerPage={this.state.itemsPerPage} 
                            totalItemsCount={this.state.totalRecords}
                            onChange={this.handlePaging} />
              </CardBody>
            </Card>
          </Col>
        </Row>
        <Modal isOpen={this.state.isUpdloadOpen} >
          <ModalHeader>Upload dữ liệu phòng ban</ModalHeader>
          <ModalBody>
          <Form method="post" onSubmit={this.onFormSubmit} >
              <Row>
                <Col xs="12">
                  
                    <FormGroup row>
                        <Col md="3">
                        <Label htmlFor="file-input">Chọn file</Label>
                        </Col>
                        <Col xs="12" md="9">
                        <Input type="file" id="file-input" name="file-input"  onChange={e => this.onChange(e)} />
                        </Col>
                    </FormGroup>
          
                </Col>
                <Col>
                  {uploadbtn} {' '}
                  <Button type="button" color="secondary" onClick={this.closeModal}>Close</Button>
                </Col>
              </Row>
          </Form>
          </ModalBody>
        </Modal>
      </div>
    );
  }
}


function mapStateToProps(state) {
  return {
    modalData: state.modalData
  };
}

function mapDispatchToProps(dispatch){
  return bindActionCreators({
    toggleModal
  },dispatch);
}

export default connect(mapStateToProps, mapDispatchToProps)(Departments);
