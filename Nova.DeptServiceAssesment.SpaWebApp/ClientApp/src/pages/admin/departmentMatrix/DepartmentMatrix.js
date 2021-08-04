import React, { Component, lazy, Suspense } from 'react';
import { Spinner, Button, Badge, Card, CardBody, CardHeader, Col, Row, Table, Modal, ModalHeader, ModalBody, FormGroup, Label, Input, ModalFooter, Form } from 'reactstrap';
import { DataService } from '../../../services';


import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';


class DepartmentMatrix extends Component {
  constructor(props) {
    super(props);
    this.state = {
        maxtrix: [],
        isUpdloadOpen: false,
        isuploading: false,
        file: ''
      };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  queryData = () => {
    DataService.getDepartmentMatrix()
      .then(result => {
        this.setState({
            maxtrix: result.data
        });
      })
      .catch(error => {
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  componentDidMount = () => {
    this.queryData();
  }

  renderTableHeader = () => {
    if(this.state.maxtrix && this.state.maxtrix.length > 0){
        let header = Object.keys(this.state.maxtrix[0])
        return header.map((key, index) => {
            if(index == 0)
                return <th key={index}>{' '}</th>
            else
                return <th key={index}>{key.toUpperCase()}</th>
        })
    }
    
 }

renderTableData = () => {
    if(this.state.maxtrix && this.state.maxtrix.length > 0) {
        return this.state.maxtrix.map((dep, index) => {
            
            let propertyValues = [];            
            for (var property in dep) {
                propertyValues.push(dep[property]);
            }

            const cols = propertyValues.map((prop, i) => {
                return <td>{prop}</td>
            });

            return (
               <tr key={index}>
                  {cols}
               </tr>
            )
         })
    }
}


openModal = () => {
  this.setState({isUpdloadOpen: true});
}

closeModal = () => {
    this.setState({isUpdloadOpen: false});
}

onFormSubmit = (e) => {
  e.preventDefault() // Stop form submit
  this.setState({isuploading : true})
  DataService.updateDepartmentMatrix(this.state.file)
      .then(result => {
        this.setState({          
          file: null,
        });

        this.queryData();
        this.setState({isuploading : false, isUpdloadOpen: false});
        this.props.toggleModal(true, 'Done','Upload thành công');
      })
      .catch(error => {
        this.setState({isuploading : false, isUpdloadOpen: false});
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
}

onChange = (e) => {
  this.setState({file:e.target.files[0]});
}

render() {

  const uploadbtn = this.state.isuploading ?
            <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
            <Button type="submit" color="secondary">Submit</Button>;
  return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Bảng tương tác giữa các phòng ban
                <div className="card-header-actions">
                  <Button type="button" size="sm" color="success" className="float-right" 
                          onClick={this.openModal}><i className="fa fa-plus"></i> Import</Button>
                </div>
              </CardHeader>
              <CardBody>
                <Table hover bordered striped responsive>
                  <thead>
                  <tr>
                    {this.renderTableHeader()}
                  </tr>
                  </thead>
                  <tbody>
                    {this.renderTableData()}
                  </tbody>
                </Table>
              </CardBody>
            </Card>
          </Col>
        </Row>
        <Modal isOpen={this.state.isUpdloadOpen} >
          <ModalHeader>Upload bảng phụ thuộc các phòng ban</ModalHeader>
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
  
  export default connect(mapStateToProps, mapDispatchToProps)(DepartmentMatrix);
  

