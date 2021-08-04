import React, { Component } from 'react';
import { Button, Card, CardBody, CardHeader, Col, Row, Table,
         FormGroup, Label, Input, Spinner } from 'reactstrap';
import { DataService } from '../../../services';
import ReactHTMLTableToExcel from 'react-html-table-to-excel';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';

class Reports extends Component {
    constructor(props) {
      super(props);
      this.state = {
        periods: [],
        departments: [],
        reportType : 0,
        periodId: 0,
        departmentId: 0,
        reports: [],
        querying : false
      };
    }
  
    loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>
  
      
    componentDidMount = () => {
        this.queryPeriods();
        this.queryDepartments();
    }
  
    queryPeriods = () => {

        DataService.getAdminPeriods(1, 1000)
            .then(result => {
                this.setState({          
                    periods: result.data.data,
                });
            })
            .catch(error => {
                this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
            });

    }

    queryDepartments = () => {
        DataService.getDepartments('', 1, 1000)
          .then(result => {
            this.setState({        
              departments: result.data.data
            });
          })
          .catch(error => {
            this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
          });
      }

    queryReport = () => {
        if(this.state.reportType > 0 && this.state.periodId > 0 && 
            ((this.state.reportType === 5 && this.state.departmentId !== 0) || this.state.reportType !== 5)) {
            this.setState({          
                querying: true
            });
            DataService.getReport(this.state.reportType, this.state.periodId, this.state.departmentId)
            .then(result => {
                this.setState({          
                    reports: result.data,
                    querying: false
                });
            })
            .catch(error => {
                this.setState({          
                    querying: false
                });
                this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
            });
                
        }
    }

    renderTableHeader = () => {
        if(this.state.reports && this.state.reports.length > 0){
            let header = Object.keys(this.state.reports[0])
            return header.map((key, index) => {
                
                if(key.toUpperCase().indexOf('QUESTION')!==-1){
                    return <th key={index}>{'Câu hỏi / Tiêu chí'}</th>
                }
                else if(key.toLowerCase().indexOf('departmentid2')!==-1){
                    return <th key={index}>{'Mã phòng đánh giá'}</th>
                }
                else if(key.toLowerCase().indexOf('departmentid1')!==-1){
                    return <th key={index}>{'Mã phòng được đánh giá'}</th>
                }
                else if(key.toLowerCase().indexOf('departmentid')!==-1){
                    return <th key={index}>{'Mã phòng'}</th>
                }
                else if(key.toLowerCase().indexOf('departmentname2')!==-1){
                    return <th key={index}>{'Phòng đánh giá'}</th>
                }
                else if(key.toLowerCase().indexOf('numofassigneddepts')!==-1){
                    return <th key={index}>{'Số lượng phòng đánh giá'}</th>
                }
                else if(key.toLowerCase().indexOf('departmentname')!==-1){
                    return <th key={index}>{'Phòng'}</th>
                }
                else if(key.toLowerCase().indexOf('departmentname1')!==-1){
                    return <th key={index}>{'Phòng được đánh giá'}</th>
                }
                else if(key.toLowerCase().indexOf('tong diem')!==-1){
                    return <th key={index}>{'Tổng Điểm'}</th>
                }
                else if(key.toLowerCase().indexOf('tong binh quan')!==-1){
                    return <th key={index}>{'Điểm Bình Quân'}</th>
                }
                else if(key.toLowerCase().indexOf('divcode')!==-1){
                    return <th key={index}>{'Mã Khối'}</th>
                }
                else if(key.toLowerCase().indexOf('divname')!==-1){
                    return <th key={index}>{'Khối'}</th>
                }
                else if(key.toLowerCase().indexOf('totalcriteria1')!==-1){
                    return <th key={index}>{'Tổng điểm t=iêu thức chất lượng'}</th>
                }
                else if(key.toLowerCase().indexOf('avgcriteria1')!==-1){
                    return <th key={index}>{'Điểm bình quân tiêu thức chất lượng'}</th>
                }
                else if(key.toLowerCase().indexOf('totalcriteria2')!==-1){
                    return <th key={index}>{'Tổng điểm tiêu thức thái độ'}</th>
                }
                else if(key.toLowerCase().indexOf('avgcriteria2')!==-1){
                    return <th key={index}>{'Điểm bình quân tiêu thức thái độ'}</th>
                }
                else if(key.toLowerCase().indexOf('totalcriteria3')!==-1){
                    return <th key={index}>{'Tổng điểm tiêu thức thời gian'}</th>
                }
                else if(key.toLowerCase().indexOf('avgcriteria3')!==-1){
                    return <th key={index}>{'Điểm bình quân tiêu thức thời gian'}</th>
                }
                else if(key.toLowerCase().indexOf('avgall')!==-1){
                    return <th key={index}>{'Điểm bình quân ĐG CLDV'}</th>
                }
                else{
                    return <th key={index}>{key}</th>
                }
                    
            })
        }
        
     }
    
    renderTableData = () => {
        if(this.state.reports && this.state.reports.length > 0) {
            return this.state.reports.map((dep, index) => {
                
                let propertyValues = [];            
                for (var property in dep) {
                    propertyValues.push(dep[property]);
                }
    
                const cols = propertyValues.map((prop, i) => {
                    return <td key={i}>{prop}</td>
                });
    
                return (
                   <tr key={index}>
                      {cols}
                   </tr>
                )
             })
        }
    }

    handleChange = event => {

        const {name, value} = event.target;
        this.setState({[name] : value});
        
    }
  

    render() {
        
        const dataPeriods = this.state.periods.length === 0 ? '' :
                        this.state.periods.map((p, i) => {
                            return (
                                <option key={i+1} value={p.id}>{p.periodName}</option>
                            )
                        });
        
        const dataDepartments = this.state.departments.length === 0 ? '' :
                        this.state.departments.map((p, i) => {
                            return (
                                <option key={i+1} value={p.id}>{p.name}</option>
                            )
                        });

        const querybtn = this.state.querying ?
                        <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                        <Button type="submit" size="sm" color="primary" onClick={e => this.queryReport()} ><i className="fa fa-dot-circle-o"></i> Query</Button>;

      return (
        <div className="animated fadeIn">
          <Row>
              <Col xs="12">
              <Card>
                  <CardHeader>
                        <strong>Báo cáo</strong>
                        <div className="card-header-actions">
                            <ReactHTMLTableToExcel
                                id="table-xls-button"
                                className="btn btn-primary btn-sm float-right"
                                table="report-table"
                                filename="report"
                                sheet="tablexls"
                                buttonText="Download as XLS"/>
                    {/* <Button type="button" size="sm" color="success" className="float-right" onClick={this.openModal}><i className="fa fa-file"></i> Import</Button> */}
                        </div>
                  </CardHeader>
                  <CardBody>
                        <FormGroup row>
                            <Col xs="3" md="3">
                                <Label htmlFor="reportType">Loại báo cáo</Label>
                            </Col>
                            <Col xs="9" md="9">
                                <Input  type="select" name="reportType" id="reportType"
                                        value={this.state.reportType}
                                        onChange={e => this.handleChange(e)}>
                                    <option value='0'>Vui lòng chọn</option>
                                    <option value='1'>Báo cáo chất lượng dịch vụ các PB theo câu hỏi</option>
                                    <option value='2'>Báo cáo chất lượng dịch vụ các PB theo tiêu thức đánh giá</option>
                                    <option value='3'>Báo cáo chi tiết chất lượng dịch vụ từng PB</option>
                                    <option value='5'>Báo cáo điểm từng phòng ban</option>
                                    <option value='6'>Báo cáo điểm tổng hợp tất cả phòng ban</option>
                                    <option value='7'>Báo cáo chi tiết đánh giá của từng nhân viên</option>
                                </Input>
                            </Col>
                        </FormGroup>
                        <FormGroup row>
                            <Col xs="3" md="3">
                                <Label htmlFor="reportType">Chọn kỳ đánh giá</Label>
                            </Col>
                            <Col xs="9" md="9">
                                <Input  type="select" name="periodId" id="periodId"
                                        value={this.state.periodId}
                                        onChange={this.handleChange}>
                                    <option key="0" value='0'>Vui lòng chọn</option>
                                    {dataPeriods}
                                </Input>
                            </Col>
                        </FormGroup>
                        {
                            this.state.reportType == 5 && <FormGroup row>
                                <Col xs="3" md="3">
                                    <Label htmlFor="reportType">Chọn phòng ban</Label>
                                </Col>
                                <Col xs="9" md="9">
                                    <Input  type="select" name="departmentId" id="departmentId"
                                            value={this.state.departmentId}
                                            onChange={this.handleChange}>
                                        <option key="0" value='0'>Vui lòng chọn</option>
                                        {dataDepartments}
                                    </Input>
                                </Col>
                            </FormGroup>
                        }
                        <FormGroup row>
                            <Col xs="12">
                                {querybtn}
                            </Col>
                        </FormGroup>
                        <FormGroup row>
                            <Col xs="12">
                                <Table hover bordered striped responsive id="report-table">
                                <thead>
                                <tr>
                                    {this.renderTableHeader()}
                                </tr>
                                </thead>
                                <tbody>
                                    {this.renderTableData()}
                                </tbody>
                                </Table>
                            </Col>
                        </FormGroup>
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
  
  export default connect(mapStateToProps, mapDispatchToProps)(Reports);
  