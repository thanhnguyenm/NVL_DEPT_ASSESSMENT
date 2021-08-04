import React, { Component, lazy, Suspense } from 'react';
import { Badge, Card, CardBody, CardHeader, Col, Pagination, PaginationItem, PaginationLink, Row, Table, NavLink } from 'reactstrap';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { changeDepartmentList } from '../../../actions/changeDepartmentList';
import { toggleModal } from '../../../actions/toggleModal';
import { DataService } from '../../../services';

class Departments extends Component {
  constructor(props) {
    super(props);
    this.state = {
      departments: null
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>
  
  queryData = () => {
    DataService.getUserAssessmentDepartments(this.props.match.params.id)
      .then(result => {

        this.setState({
          departments: result.data.data
        });
        this.props.changeDepartmentList(result.data.data);
      })
      .catch(error => {
        //console.log(error);
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  componentDidMount = () => {
    this.queryData();
  }

  render() {

    const dataRows =  this.state.departments == null ? 
                        <tr>
                          <td colSpan="4" align="center">Loading</td>
                        </tr> :
                      this.state.departments.length === 0 ? 
                        <tr>
                          <td colSpan="4" align="center">Không có phòng ban nào đượcchọn</td>
                        </tr> :
                        this.state.departments.map((p, i) => {
                          
                          const finishIcon = p.finished ?  <i className="cui-circle-check icons"></i> :
                                                           <i className="cui-circle-x icons"></i>;

                          const icon = p.status == 'PENDING' ? <i className="cui-circle-x icons"></i> :
                                      p.status == 'COMPLETE' ? <i className="cui-circle-check icons"></i> : null;

                          return (
                              <tr key={p.id}>
                                <td align="center">{i+1}</td>
                                <td><a href={`#/user/assessments/${this.props.match.params.id}/departments/${p.id}`}>{p.departmentName}</a></td>
                                <td align="center">{finishIcon}</td>
                                <td align="center">{icon}</td>
                              </tr>
                          )
                        });

    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Danh sách các phòng ban đánh giá
              </CardHeader>
              <CardBody>
                <Table hover bordered striped responsive size="sm">
                  <thead>
                  <tr>
                    <th width="50">STT</th>
                    <th>Phòng ban</th>
                    <th width="100">Hoàn thành</th>
                    <th width="100">Trạng thái</th>
                  </tr>
                  </thead>
                  <tbody>
                    {dataRows}
                  </tbody>
                </Table>
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
    assessmentDepartmentData : state.assessmentDepartmentData
  };
}

function mapDispatchToProps(dispatch){
  return bindActionCreators({
    changeDepartmentList,
    toggleModal
  },dispatch);
}


export default connect(mapStateToProps,mapDispatchToProps)(Departments);

