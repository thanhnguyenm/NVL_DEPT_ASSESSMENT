import React, { Component } from 'react';
import {
    Badge,
    Button,
    Card,
    CardBody,
    CardFooter,
    CardHeader,
    Col,
    Form,
    FormGroup,
    Input,
    Label,
    Row,
    Spinner
  } from 'reactstrap';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../../actions/toggleModal';
import { DataService } from '../../../services';


class AddQuestions extends Component {
    constructor(props) {
        super(props);
        this.state = {
            questionId: 0,
            criteriaId: 0,
            content: '',
            criterias: [],
            saving : false
        };
    }

    loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

    handleChange = (event) => {

        const {id, name, value, type} = event.target;
    
        this.setState({ [name]: value });
    }
    
    queryData = () => {
        DataService.getQuestionCriteria()
          .then(result => {
            this.setState({
                criterias: result.data.data
            });
          })
          .catch(error => {
            //console.log(error);
            this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
          });
    }

    loadEdit = (id) => {
        DataService.getQuestion(id)
          .then(result => {
            this.setState({
                questionId: result.data.id,
                criteriaId: result.data.criteriaId,
                content: result.data.content
            });
          })
          .catch(error => {
            //console.log(error);
            this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
          });
    }

    formReset = () => {
        
        this.setState({
            criteriaId: 0,
            content: ''
        });
    }

    handleSubmit = (e) => {
        e.preventDefault();

        if(!this.state.criteriaId || this.state.criteriaId == 0 || !this.state.content){
            this.props.toggleModal(true,'Lỗi', 'Không được để trống dữ liệu');
            return;
        }

        this.setState({saving : true})

        let datapost = {
            id: this.state.questionId,
            criteriaId: this.state.criteriaId,
            content: this.state.content
        }

        DataService.saveQuestion(datapost)
            .then(result => {
                this.setState({saving: false, questionId: result.data});
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


    componentDidMount = () => {
    
        this.queryData();

        if(this.props.match.params.id){
            this.loadEdit(this.props.match.params.id);
        }

    }

    render() {

        const selectList = this.state.criterias.map((c, i) => {

            return <option key={c.id} value={c.id}>{c.criteriaName}</option>

        });

        const savebtn = this.state.saving ?
                            <Button variant="primary" disabled><Spinner as="span" animation="grow" size="sm" role="status" aria-hidden="true" /> Loading...</Button> :
                            <Button type="submit" size="sm" color="primary" onClick={e => this.handleSubmit(e)} ><i className="fa fa-dot-circle-o"></i> Lưu</Button>;

        return (
            <div className="animated fadeIn">
                <Row>
                    <Col xs="12">
                    <Card>
                        <CardHeader>
                            <strong>Thêm / Sửa Câu Hỏi</strong>
                        </CardHeader>
                        <CardBody>
                            <Form action="" method="post" className="form-horizontal">
                            <FormGroup row>
                                <Col md="3">
                                    <Label htmlFor="selectLg">Tiêu chuẩn</Label>
                                </Col>
                                <Col xs="12" md="9" size="lg">
                                    <Input  type="select" name="criteriaId" id="criteriaId" bsSize="lg" 
                                            value={this.state.criteriaId}
                                            onChange={this.handleChange}>
                                        <option value='0'>Vui lòng chọn</option>
                                        {selectList}
                                    </Input>
                                </Col>
                            </FormGroup>
                            <FormGroup row>
                                <Col md="3">
                                <Label htmlFor="text-input">Nội dung</Label>
                                </Col>
                                <Col xs="12" md="9" size="lg">
                                <Input type="text" id="content" name="content" placeholder="Text" bsSize="lg"
                                        value={this.state.content}
                                        onChange={this.handleChange}/>
                                {/* <FormText color="muted">This is a help text</FormText> */}
                                </Col>
                            </FormGroup>
                            </Form>
                        </CardBody>
                        <CardFooter>
                            {savebtn}{' '}
                            <Button disabled={this.state.saving} type="reset" size="sm" color="danger" onClick={this.formReset}><i className="fa fa-ban"></i> Reset</Button>
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
      
    };
  }
  
  
  function mapDispatchToProps(dispatch){
    return bindActionCreators({
      toggleModal
    },dispatch);
  }
  
  export default connect(mapStateToProps,mapDispatchToProps)(AddQuestions);
  
