import React, { Component } from 'react';
import { Button, Card, CardBody, CardHeader, Col, Row, Table, Form, FormGroup, Label, Input, FormText} from 'reactstrap';
import { DataService } from '../../../services';
import { CustomPagination } from '../../../components';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';

class Permissions extends Component {
  constructor(props) {
    super(props);
    this.state = {
      permissions: null,
      saving: false,
      newEmail: '',
      activePage: 1,
      itemsPerPage: 50
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  queryData = (activePage) => {
    DataService.getPermissions(activePage, this.state.itemsPerPage)
      .then(result => {
        this.setState({          
          permissions: result.data.data,
          activePage : activePage,
          totalRecords: result.data.totalRecords
        });
      })
      .catch(error => {
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  componentDidMount = () => {
    this.queryData(1);
  }

  handlePaging = (number) => {
    
    if(number !== this.state.activePage){
      this.setState({          
        permissions: null
      });
      this.queryData(number);
    }

  }

  handleChange = (event) => {

    const {name, value} = event.target;

    this.setState({ [name]: value });
    
  }

  handleSubmit = (e) => {
    e.preventDefault();
    
    if(!this.state.newEmail) {
        this.props.toggleModal(true, 'Lỗi','Vui lòng nhập email');
        return;
    }

    this.setState({saving : true})

    DataService.savePermission(this.state.newEmail)
            .then(result => {
                this.setState({saving: false, newEmail: ''});
                this.queryData(1);
                this.props.toggleModal(true, 'Done','Lưu dữ liệu thành công');
            })
            .catch(error => {
                this.setState({saving : false});
                let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
                if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
                msg = error.response.data.errors.domainValidations[0];
                this.props.toggleModal(true, 'Lỗi', msg);
            });

  }

  deletePermission = (email) => {
    DataService.deletePermission(email)
            .then(result => {
                if(result.data){
                    this.queryData(1);
                    this.props.toggleModal(true, 'Done','Xóa dữ liệu thành công');
                } else {
                    this.props.toggleModal(true, 'Done','Không thể xóa dữ liệu');
                }
                
            })
            .catch(error => {
                let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
                if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
                msg = error.response.data.errors.domainValidations[0];
                this.props.toggleModal(true, 'Lỗi', msg);
            });
  }


  render() {

    let dataRows =  this.state.permissions == null ?
                    <tr>
                      <td colSpan="5" align="center">Loading</td>
                    </tr> :
                    this.state.permissions.length === 0 ?
                    <tr>
                      <td colSpan="5" align="center">Không có dữ liệu</td>
                    </tr> :
                    this.state.permissions.map((p, i) => {
                      return (
                          <tr key={i}>
                            <td>{(this.state.activePage-1)*this.state.itemsPerPage+i+1}</td>
                            <td>{p.email}</td>
                            <td><Button type="submit" size="sm" color="success" className="float-right" onClick={e => this.deletePermission(p.email)}><i className="cui-circle-check icons"></i> Xóa</Button></td>
                          </tr>
                      )
                    });


    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Danh sách administrators
                <div className="card-header-actions">
                </div>
              </CardHeader>
              <CardBody>
                <Form action="" method="post" className="form-horizontal" noValidate onSubmit={e => this.handleSubmit(e)}>
                <FormGroup row>
                    <Col xs="3" md="2">
                    <Label htmlFor="text-input">Email</Label>
                    </Col>
                    <Col xs="6" md="8">
                    <Input type="email" id="newEmail" name="newEmail" placeholder="Text"
                            value={this.state.newEmail} required
                            onChange={e => this.handleChange(e)}/>
                    <FormText color="muted">ex: email1@novaland.com;email2@novaland.com;....</FormText>
                    </Col>
                    <Col xs="3" md="2">
                        <Button type="submit" size="sm" color="success" className="float-right"><i className="fa fa-plus"></i> Thêm</Button>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Col xs="12">

                    </Col>
                </FormGroup>
                </Form>

                <Table hover bordered striped responsive>
                  <thead>
                  <tr>
                    <th width="80">STT</th>
                    <th>Email</th>
                    <th width="100">&nbsp;</th>
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

export default connect(mapStateToProps, mapDispatchToProps)(Permissions);
