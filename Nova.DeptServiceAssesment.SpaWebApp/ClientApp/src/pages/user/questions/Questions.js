import React, { Component } from 'react';
import { Card, CardBody, CardHeader, CardFooter, Col,  Row, Table, Button, Input, Spinner } from 'reactstrap';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { changeDepartmentList } from '../../../actions/changeDepartmentList';
import { toggleModal } from '../../../actions/toggleModal';
import { DataService } from '../../../services';

class Questions extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      questions: null,
      dirty: false,
      saving: false,
      finishing: false,
      canedit: false,
      periodId: 0, 
      deptId: 0
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  queryData = (periodId, deptId) => {
    if(!periodId) periodId = this.state.periodId;
    if(!deptId) deptId = this.state.deptId;

    DataService.getUserAssessmentQuestions(periodId, deptId)
      .then(result => {
        
        //can edit data ??
        let canedit = false;
        if(result.data.data && result.data.data.length > 0){
          canedit = result.data.data[0].canEdit;
        }
        
        
        this.setState({
          questions: result.data.data,
          canedit : canedit,
          periodId: periodId, 
          deptId: deptId
        });

      })
      .catch(error => {
        //console.log(error);
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  queryDepartmentData = () => {
    DataService.getUserAssessmentDepartments(this.props.match.params.id)
      .then(result => {
        this.props.changeDepartmentList(result.data.data);
      })
      .catch(error => {
        //console.log(error);
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  handleChange = (event) => {

    const {name, value, type} = event.target;

    this.setState((prevState) => {
      
      let copyState = {...prevState};
      let questions = copyState.questions.map((q, i)=>{

        if(type == "radio"){
          if(name == `question_${q.periodQuetionId}`){
            q.result = value;
          }
        } else {
          if(name == `txt_${q.periodQuetionId}-reason`){
            q.resultComment = value;
          }
        }
        return q;
      });

      copyState.questions = questions;
      copyState.dirty = true;

      
      return copyState;
    });
    
  }

  checkAll = (rs) => {
    this.setState((prevState) => {
      
      let copyState = {...prevState};
      let questions = copyState.questions.map((q, i)=>{
        q.result = rs;
        return q;
      });

      copyState.questions = questions;
      copyState.dirty = true;

      return copyState;
    });
  }

  handleNanigation = (id) => {
    let depid = this.props.assessmentDepartmentData.departments[id].id;
    window.location.assign(`/#/user/assessments/${this.state.periodId}/departments/${depid}`);
    //window.location.reload();
    this.queryData(this.props.match.params.id, depid);
  }

  saveData = (event, isComplete) => {
    event.preventDefault();

    if(isComplete)  this.setState({finishing : true}); else  this.setState({saving : true});
    let datapost = {
      periodId: parseInt(this.props.match.params.id),
      periodSelectedDepartmentId: parseInt(this.props.match.params.depid),
      result : [...this.state.questions]
    }
    DataService.saveAssessmentResult(datapost, isComplete)
      .then(result => {
        if(isComplete)  this.setState({finishing : false}); else  this.setState({saving : false});
        this.queryData();
        this.props.toggleModal(true, 'Done','Lưu dữ liệu thành công');
      })
      .catch(error => {
        if(isComplete)  this.setState({finishing : false}); else  this.setState({saving : false});
        let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
        if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
          msg = error.response.data.errors.domainValidations[0];
        this.props.toggleModal(true, 'Lỗi', msg);
      });
  }


  componentDidMount = () => {
    this.queryData(this.props.match.params.id, this.props.match.params.depid);
    
    if(this.props.assessmentDepartmentData.departments == null){
      this.queryDepartmentData();
    }

  }

  componentDidUpdate = () => {
    
  }

  render() {
    
    //render data
    const dataRows =    this.state.questions == null ? 
                          <tr>
                            <td colSpan="10" align="center">Loading</td>
                          </tr> :
                        this.state.questions.length == 0 ? 
                          <tr>
                            <td colSpan="10" align="center">Không có câu hỏi đánh giá nào</td>
                          </tr> :
    
                          this.state.questions.map((p, i) => {
                            const results = [0,1,2,3,4,5];
                            const radios = results.map((rs, j) => {
                              
                              const rad = <Input  className="form-check-input" type="radio" id={`rad_question_${p.periodQuetionId}-radio${rs}`} 
                                                  name={`question_${p.periodQuetionId}`} value={rs} 
                                                  checked={ p.result == rs } 
                                                  onChange={e => this.handleChange(e)}
                                                  disabled={!this.state.canedit}
                                                  /> ;  

                              return (
                                <td align="center" key={j}>
                                  { rad }
                                </td>
                              )
                            });

                            return (
                              <tr key={i}>
                                <td align="center">{i+1}</td>
                                <td>{p.criteriaName}</td>
                                <td>{p.questionContent}</td>
                                {radios}
                                <td align="center">
                                  <Input type="text" id={`txt_${p.periodQuetionId}-reason`} name={`txt_${p.periodQuetionId}-reason`} 
                                         value={!p.resultComment ? '' : p.resultComment}
                                         onChange={e => this.handleChange(e)}
                                         disabled={!this.state.canedit}/>
                                </td>
                              </tr>
                            )
                          });

    
    const checkAll0Btn = <i className="cui-circle-check icons" onClick={e => this.checkAll(0)}></i>;

    //render buttons
    let [first, prev, current, next, last] = [0, 0, 0, 0, 0];
    let depName = this.props.match.params.depid;

    if(this.props.assessmentDepartmentData.departments){
      first = 0;
      last = Math.max(this.props.assessmentDepartmentData.departments.length - 1, 0);
      for(let i=0; i<this.props.assessmentDepartmentData.departments.length; i++){
        if(this.props.assessmentDepartmentData.departments[i].id == this.props.match.params.depid){
          current = i;
          break;
        }
      }

      prev = Math.max(current - 1, 0);;
      next = Math.min(current + 1, this.props.assessmentDepartmentData.departments.length - 1) ;
      depName = this.props.assessmentDepartmentData.departments[current].departmentName;
    }

    const firstBtn = first != null && first != current ? <Button type="button" size="sm" color="success" onClick={e => this.handleNanigation(first)} ><i className="fa fa-fast-backward"></i> </Button> : '';
    const prevBtn = prev != null && prev != current ? <Button type="button" size="sm" color="success" onClick={e => this.handleNanigation(prev)}><i className="fa fa-backward"></i></Button> : '';
    const saveBtn = this.state.saving ?
                        <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                        <Button type="button" size="sm" color="success" onClick={(e) => this.saveData(e, false)} disabled={!this.state.canedit}><i className="fa fa-dot-circle-o"></i> Lưu</Button>;
    const finishBtn = this.state.finishing ?
                        <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                        <Button type="button" size="sm" color="success" onClick={(e) => this.saveData(e, true)} disabled={!this.state.canedit}><i className="fa fa-dot-circle-o"></i> Hoàn thành</Button>;
    const nextBtn = next != null && next != current ? <Button type="button" size="sm" color="success" onClick={e => this.handleNanigation(next)}><i className="fa fa-forward"></i></Button> : '';
    const LastBtn = last != null && last != current ? <Button type="button" size="sm" color="success" onClick={e => this.handleNanigation(last)}><i className="fa fa-fast-forward"></i></Button> : '';

    
    return (
      <div className="animated fadeIn">
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <i className="fa fa-align-justify"></i> Đánh giá chất lượng dịch vụ Phòng <strong>{depName}</strong>
              </CardHeader>
              <CardBody>
                <Table hover bordered striped responsive size="sm" className="center-table">
                  <thead>
                  <tr>
                    <th width="30" className="center-td">STT</th>
                    <th width="100" className="center-td">Tiêu thức</th>
                    <th className="center-td">Nội dung</th>
                    <th width="50" className="center-td">Không đánh giá {checkAll0Btn}</th>
                    <th width="50" className="center-td">Hoàn toàn không hài lòng</th>
                    <th width="50" className="center-td">Không hài lòng</th>
                    <th width="50" className="center-td">Binh thường</th>
                    <th width="50" className="center-td">Hài lòng</th>
                    <th width="50" className="center-td">Hoàn toàn hài lòng</th>
                    <th width="150" className="center-td">Lý do</th>
                  </tr>
                  </thead>
                  <tbody>
                    {dataRows}
                  </tbody>
                </Table>
              </CardBody>
              <CardFooter>
                {firstBtn} {prevBtn} {saveBtn} {finishBtn} {nextBtn} {LastBtn}
              </CardFooter>
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

export default connect(mapStateToProps,mapDispatchToProps)(Questions);

