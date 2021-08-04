import React, { Component, lazy, Suspense } from 'react';
import { Button, Badge, Card, CardBody, CardHeader, Col, Row, Table } from 'reactstrap';
import { DataService } from '../../../services';
import { CustomPagination } from '../../../components';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';

class AdminQuestions extends Component {
  constructor(props) {
    super(props);
    this.state = {
      questions: null,
      activePage: 0,
      itemsPerPage: 10,
      totalRecords: 0
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  queryData = (activePage) => {
    DataService.getAdminQuestions(activePage, this.state.itemsPerPage)
      .then(result => {
        this.setState({          
          questions: result.data.data,
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
    
    if(number != this.state.activePage){
      this.queryData(number);
    }

  }

  navToAddNewPage = () => {
    this.props.history.push('/admin/questions/add')
  }

  render() {

    let dataRows =  this.state.questions == null ?
                    <tr>
                      <td colSpan="4" align="center">Loading</td>
                    </tr> :
                    this.state.questions.length === 0 ?
                    <tr>
                      <td colSpan="4" align="center">Không có dữ liệu</td>
                    </tr> :
                    this.state.questions.map((p, i) => {
                      return (
                          <tr key={p.id}>
                            <td>{p.id}</td>
                            <td>{p.criteriaName}</td>
                            <td><a href={`#/admin/questions/edit/${p.id}`}>{p.content}</a></td>
                          </tr>
                      )
                    });

    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Quản lý các câu hỏi đánh giá phòng ban
                <div className="card-header-actions">
                  <Button type="button" size="sm" color="success" className="float-right" onClick={this.navToAddNewPage}><i className="fa fa-plus"></i> Thêm</Button>
                </div>
              </CardHeader>
              <CardBody>
                <Table hover bordered striped responsive>
                  <thead>
                  <tr>
                    <th width="80">STT</th>
                    <th width="200">Tiêu thức</th>
                    <th>Nội dung</th>
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

export default connect(mapStateToProps, mapDispatchToProps)(AdminQuestions);
