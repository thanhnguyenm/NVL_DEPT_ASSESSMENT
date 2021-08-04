import React, { Component } from 'react';
import { Button,  Card, CardBody, CardHeader, Col, Row, Table,
         Form, FormGroup, Label, Input, Spinner } from 'reactstrap';
import { DataService } from '../../../services';
import { CustomPagination } from '../../../components';
import moment from 'moment';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';

class EditPeriod extends Component {
  constructor(props) {
    super(props);
    this.state = {
      periodId: 0,
      periodName: '',
      periodFrom: '',
      periodTo: '',
      published: false,
      departments: [],
      questions: [],
      selectedQuestions: [],
      saving : false,
      notifying : false,
      remindering : false,
      activePage: 1,
      itemsPerPage: 50,
      totalRecords: 0,
      activePageQuestion: 1,
      itemsPerPageQuestion: 50,
      totalRecordsQuestion: 0
    };
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  loadEdit = (id) => {
    DataService.getAdminPeriod(id)
      .then(result => {
          
        this.setState({
            periodId: result.data.id,
            periodName: result.data.periodName,
            periodFrom: result.data.periodFrom,
            periodTo: result.data.periodTo,
            published: result.data.published,
            questions: [],
            selectedQuestions: result.data.questions,
            departments: [],
            canGenerated: false
        });

        this.queryDepartments(id, 1);
        this.queryAllQuestions(1);
      })
      .catch(error => {
        
        this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
 }

  queryDepartments = (id, activePage) => {
    DataService.getAdminPeriodDepartments(id,activePage, this.state.itemsPerPage)
      .then(result => {
        this.setState({          
            departments: result.data.data,
            activePage : activePage,
            totalRecords: result.data.totalRecords
        });
      })
      .catch(error => {
        //this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  queryAllQuestions = (activePageQuestion) => {
    DataService.getAdminQuestions(activePageQuestion, this.state.itemsPerPageQuestion)
      .then(result => {
        this.setState({          
            questions: result.data.data,
            activePageQuestion : activePageQuestion,
            totalRecordsQuestion: result.data.totalRecords
        });
      })
      .catch(error => {
        //this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  componentDidMount = () => {
    if(this.props.match.params.id){
        this.loadEdit(this.props.match.params.id);
    } else{
      this.queryAllQuestions(1);
    }


    
  }

  handleChange = (event, id) => {
    const {name, value, type, checked} = event.target;

    if(type === "checkbox" && name === "published") 
      this.setState({[name] : checked});
    else if(type === "checkbox" && name !== "published" && id){
      this.setState((prev) => {
        
        const oldStates = {...prev};
        
        for( var i = 0; i < oldStates.selectedQuestions.length; i++)
        { 
          
            if ( oldStates.selectedQuestions[i] === id) { 

              oldStates.selectedQuestions.splice(i, 1);

            }
        }
        if(checked) {
          oldStates.selectedQuestions.push(id);
        }

        return oldStates;

      });
    } else {
      this.setState({[name] : value});
    }

  }

  handleSubmit = (e) => {
    e.preventDefault();

    if(!this.state.periodName){
        this.props.toggleModal(true,'Lỗi', 'Không được để trống dữ liệu');
        return;
    }

    var from = new Date(this.state.periodFrom);
    var to = new Date(this.state.periodTo);

    if(from >= to){
        this.props.toggleModal(true,'Lỗi', 'Từ ngày phải nhỏ đến ngày');
        return;
    }

    this.setState({saving : true})

    let datapost = {
        id: this.state.periodId,
        periodName: this.state.periodName,
        periodFrom: from,
        periodTo: to,
        published: this.state.published,
        selectedQuestions: this.state.selectedQuestions
    }

    DataService.saveAdminPeriod(datapost)
        .then(result => {
            this.setState({saving: false, periodId: result.data});
            this.props.toggleModal(true, 'Done','Lưu dữ liệu thành công');
            this.props.history.push(`/admin/periods/edit/${result.data}`);
        })
        .catch(error => {
            this.setState({saving : false});
            let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
            if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
            msg = error.response.data.errors.domainValidations[0];
            this.props.toggleModal(true, 'Lỗi', msg);
        });
  }

  notify = (e) => {
    this.setState({notifying : true});

    let datapost = {
      id: this.state.periodId
    }

    DataService.notifyPeriod(datapost)
        .then(result => {
            this.setState({notifying: false});
            if(result.data)
              this.props.toggleModal(true, 'Done','Đã gửi thông báo thành công');
            else 
              this.props.toggleModal(true, 'Lỗi','Không thể gửi thông báo, vui lòng thử lại sau');
        })
        .catch(error => {
            this.setState({notifying: false});
            let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
            if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
            msg = error.response.data.errors.domainValidations[0];
            this.props.toggleModal(true, 'Lỗi', msg);
        });
  }

  reminder = (e) => {
    this.setState({remindering : true});

    let datapost = {
      id: this.state.periodId
    }

    DataService.remindPeriod(datapost)
        .then(result => {
            this.setState({remindering: false});
            if(result.data)
              this.props.toggleModal(true, 'Done','Đã gửi nhắc nhở thành công');
            else 
              this.props.toggleModal(true, 'Lỗi','Không thể gửi nhắc nhở, vui lòng thử lại sau');
        })
        .catch(error => {
            this.setState({remindering: false});
            let msg = 'Hệ thống không thể lưu dữ liệu, vui lòng liên hệ Help Desk';
            if(error.response && error.response.data && error.response.data.errors && error.response.data.errors.domainValidations && error.response.data.errors.domainValidations.length > 0)
            msg = error.response.data.errors.domainValidations[0];
            this.props.toggleModal(true, 'Lỗi', msg);
        });
  }

  

  handlePaging = (number, e) => {
    e.preventDefault();
    if(number !== this.state.activePage){
      this.queryDepartments(this.props.match.params.id, number);
    }
  }

  handlePagingQuestion = (number, e) => {
    e.preventDefault();
    if(number !== this.state.activePageQuestion){
      this.queryAllQuestions(number);
    }
  }

  checkContainsAll= () => {

    let rs = true;
    this.state.questions.forEach(q => rs = rs && this.state.selectedQuestions.includes(q.id));
    
    return rs;

  }

  checkAllQuestion = () => {

    this.setState(prevState => {

      const copyState = {...prevState};
      
      let questionNotExists = copyState.questions.filter((q,i) => !copyState.selectedQuestions.includes(q.id));
      questionNotExists.forEach(q => copyState.selectedQuestions.push(q.id));

      return copyState;

    });
    
  }

  render() {

    let dataDepts = this.state.departments.length === 0 ?
                    <tr>
                      <td colSpan="4" align="center">Loading</td>
                    </tr> :
                    this.state.departments.map((p, i) => {
                        return (
                            <tr key={i}>
                              <td>{(this.state.activePage-1)*this.state.itemsPerPage+i+1}</td>
                              <td>{p.departmentFromName}</td>
                              <td>{p.fullName}</td>
                              <td>{p.departmentToName}</td>
                            </tr>
                        )
                      });

    
    let dataQuetions = this.state.questions.length === 0 ?
                      <tr>
                        <td colSpan="4" align="center">Loading</td>
                      </tr> :
                      this.state.questions.map((p, i) => {
                        return (
                            <tr key={p.id}>
                              <td><input type="checkbox" disabled={this.state.periodId > 0}
                                        name={`question_${p.id}`}
                                        id={`question_${p.id}`} checked={this.state.selectedQuestions.includes(p.id)} 
                                        onChange={e => this.handleChange(e, p.id)} /></td>
                              <td>{p.id}</td>
                              <td>{p.content}</td>
                            </tr>
                        )
                      });

    const checkAll = <input type="checkbox" disabled={this.state.periodId > 0}
                        name={`question_check_all`}
                        id={`question_check_all`} checked={this.checkContainsAll()} 
                        onChange={e => this.checkAllQuestion(e)} />

    const savebtn = this.state.saving ?
                        <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                        <Button type="submit" size="sm" color="primary" onClick={e => this.handleSubmit(e)} ><i className="fa fa-dot-circle-o"></i> Lưu</Button>;
    const notifybtn = this.state.notifying ?
                        <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                        <Button type="submit" size="sm" color="primary" onClick={e => this.notify(e)} ><i className="fa fa-dot-circle-o"></i> Gửi thông báo</Button>;
    const reminderbtn = this.state.remindering ?
                        <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                        <Button type="submit" size="sm" color="primary" onClick={e => this.reminder(e)} ><i className="fa fa-dot-circle-o"></i> Nhắc nhở</Button>;

    return (
      <div className="animated fadeIn">
        <Row>
            <Col xs="12">
            <Card>
                <CardHeader>
                    <strong>Thêm / Sửa Kỳ Đánh Giá</strong>
                </CardHeader>
                <CardBody>
                    <Form action="" method="post" className="form-horizontal">
                    <FormGroup row>
                        <Col xs="12" md="3">
                        <Label htmlFor="text-input">Nội dung kỳ đánh giá</Label>
                        </Col>
                        <Col xs="12" md="9" size="lg">
                        <Input type="text" id="periodName" name="periodName" placeholder="Text" bsSize="lg"
                                value={this.state.periodName}
                                onChange={this.handleChange}/>
                        {/* <FormText color="muted">This is a help text</FormText> */}
                        </Col>
                    </FormGroup>
                    <FormGroup row>
                        <Col xs="12" md="3"><Label>Publish</Label></Col>
                        <Col xs="12" md="9" size="lg">
                        <FormGroup check className="checkbox">
                            <Input className="form-check-input" type="checkbox" id="published" name="published" bsSize="lg"
                                    checked={this.state.published === true}
                                    onChange={this.handleChange} />
                        </FormGroup>
                        </Col>
                    </FormGroup>
                    <FormGroup row>
                        <Col xs="12" md="3">
                            <Label htmlFor="date-input">Từ ngày</Label>
                        </Col>
                        <Col xs="12" md="3">
                            <Input type="date" id="periodFrom" name="periodFrom" placeholder="date" 
                                    value={moment(this.state.periodFrom).format("YYYY-MM-DD")}
                                    onChange={this.handleChange}/>
                        </Col>
                        <Col xs="12" md="3">
                            <Label htmlFor="date-input">Đến ngày</Label>
                        </Col>
                        <Col xs="12" md="3">
                            <Input type="date" id="periodTo" name="periodTo" placeholder="date" 
                                    value={moment(this.state.periodTo).format("YYYY-MM-DD")}
                                    onChange={this.handleChange} />
                        </Col>
                    </FormGroup>
                    <FormGroup row>
                        <Col xs="12">
                            {savebtn} {' '} {notifybtn} {' '} {reminderbtn}
                        </Col>
                    </FormGroup>
                    <FormGroup row>
                        <Col xd="12">
                            <Label htmlFor="selectLg">Chọn câu hỏi</Label>
                        </Col>
                        <Col xs="12">
                            <Table hover bordered striped responsive size="sm">
                                <thead>
                                <tr>
                                    <th width="50">{checkAll}</th>
                                    <th>Id</th>
                                    <th>Câu hỏi</th>
                                    
                                </tr>
                                </thead>
                                <tbody>
                                    {dataQuetions}
                                </tbody>
                            </Table>
                            <CustomPagination activePage={this.state.activePageQuestion} 
                            itemsPerPage={this.state.itemsPerPageQuestion} 
                            totalItemsCount={this.state.totalRecordsQuestion}
                            onChange={this.handlePagingQuestion} />
                        </Col>
                    </FormGroup>
                    <FormGroup row>
                        <Col xd="12">
                            <Label htmlFor="selectLg">Danh sách phòng ban</Label>
                        </Col>
                        <Col xs="12">
                            <Table hover bordered striped responsive size="sm">
                                <thead>
                                <tr>
                                    <th width="50">STT</th>
                                    <th>Phòng ban đánh giá</th>
                                    <th>Nhân viên đánh giá</th>
                                    <th>Phòng ban được đánh giá</th>
                                </tr>
                                </thead>
                                <tbody>
                                    {dataDepts}
                                </tbody>
                            </Table>
                            <CustomPagination activePage={this.state.activePage} 
                            itemsPerPage={this.state.itemsPerPage} 
                            totalItemsCount={this.state.totalRecords}
                            onChange={this.handlePaging} />
                        </Col>
                    </FormGroup>
                    </Form>
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

export default connect(mapStateToProps, mapDispatchToProps)(EditPeriod);
