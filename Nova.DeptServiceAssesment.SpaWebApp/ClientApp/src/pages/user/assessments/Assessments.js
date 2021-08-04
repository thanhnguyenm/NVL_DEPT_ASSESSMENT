import React, { Component, lazy, Suspense } from 'react';
import { Badge, Card, CardBody, CardHeader, Col, Row, Table } from 'reactstrap';
import { DataService } from '../../../services';
import { CustomPagination } from '../../../components';
import Moment from 'react-moment';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';


class Assessments extends Component {
  constructor(props) {
    super(props);
    this.state = {
      periods: null,
      activePage: 1,
      itemsPerPage: 10,
      totalRecords: 0
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  queryData = (activePage) => {
    DataService.getUserPeriods(activePage, this.state.itemsPerPage)
      .then(result => {
        this.setState({
          periods: result.data.data,
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

  render() {

    let dataRows = this.state.periods === null ?
                      <tr>
                        <td colSpan="4" align="center">Loading</td>
                      </tr> :
                   this.state.periods.length === 0 ?
                    <tr>
                      <td colSpan="4" align="center">Không có đợt đánh giá nào cho bạn</td>
                    </tr> :
                    this.state.periods.map((p, i) => {
                      return (
                          <tr key={p.id}>
                            <td><a href={`#/user/assessments/${p.id}`}>{p.periodName}</a></td>
                            <td><Moment format="DD/MM/YYYY">{p.periodFrom}</Moment></td>
                            <td><Moment format="DD/MM/YYYY">{p.periodTo}</Moment></td>
                            <td>
                              <Badge color="success">{p.status}</Badge>
                            </td>
                          </tr>
                      )
                    });

    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Danh sách các kỳ đánh giá
              </CardHeader>
              <CardBody>
                <Table hover bordered striped responsive>
                  <thead>
                  <tr>
                    <th>Nội dung</th>
                    <th>Từ ngày</th>
                    <th>Đến ngày</th>
                    <th>Trạng thái</th>
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

export default connect(mapStateToProps, mapDispatchToProps)(Assessments);

