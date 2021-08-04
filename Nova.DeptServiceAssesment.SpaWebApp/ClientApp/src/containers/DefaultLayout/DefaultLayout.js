import React, { Component, Suspense } from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import * as router from 'react-router-dom';
import { Container } from 'reactstrap';
import { Button, Modal, ModalBody, ModalFooter, ModalHeader } from 'reactstrap';

import {
  AppAside,
  AppFooter,
  AppHeader,
  AppSidebar,
  AppBreadcrumb2 as AppBreadcrumb,
  AppSidebarNav2 as AppSidebarNav,
} from '@coreui/react';

// sidebar nav config
import userNavigation from '../../_userNav';
import adminNavigation from '../../_adminNav';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toggleModal } from '../../actions/toggleModal';

import { DataService } from '../../services';

// routes config
import routes from '../../routes';

const DefaultAside = React.lazy(() => import('./DefaultAside'));
const DefaultFooter = React.lazy(() => import('./DefaultFooter'));
const DefaultHeader = React.lazy(() => import('./DefaultHeader'));

class DefaultLayout extends Component {

  constructor(props){
      super(props);
      this.state = {
        isAdmin: false
      }
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  closeModal = () => {
    this.props.toggleModal(false, '');
  }

  checkPermision = () => {
    DataService.checkPermission()
      .then(result => {
        this.setState({          
          isAdmin: result.data,
        });
        console.log(result.data);
      })
      .catch(error => {
        //this.props.toggleModal(true, 'Lỗi','Hệ thống không thể tải dữ liệu, vui lòng liên hệ Help Desk');
      });
  }

  componentDidMount = () => {
    this.checkPermision();
  }

  render() {
    
    const _copyPros = {...this.props};
    delete _copyPros.authenticationData;
    delete _copyPros.modalData;
    delete _copyPros.dispatch;
    delete _copyPros.toggleModal;
    
    const navigation = !this.props.authenticationData.aadResponse ? 
                          null :
                        this.state.isAdmin ? 
                          <AppSidebarNav navConfig={adminNavigation} {..._copyPros} router={router}/> :
                          <AppSidebarNav navConfig={userNavigation} {..._copyPros} router={router}/>;

    return (
      <div className="app">
        <AppHeader fixed>
          <Suspense  fallback={this.loading()}>
            <DefaultHeader {...this.props} />
          </Suspense>
        </AppHeader>
        <div className="app-body">
          <AppSidebar fixed display="lg">
            <Suspense>
              {navigation}
            </Suspense>
          </AppSidebar>
          <main className="main">
            <AppBreadcrumb appRoutes={routes} router={router}/>
            <Container fluid>
              <Suspense fallback={this.loading()}>
                <Switch>
                  {routes.map((route, idx) => {
                    return route.component ? (
                      <Route
                        key={idx}
                        path={route.path}
                        exact={route.exact}
                        name={route.name}
                        render={props => (
                          <route.component {...props} />
                        )} />
                    ) : (null);
                  })}
                  <Redirect from="/" to="/dashboard" />
                </Switch>
              </Suspense>
            </Container>
          </main>
          <AppAside fixed>
            <Suspense fallback={this.loading()}>
              <DefaultAside />
            </Suspense>
          </AppAside>
        </div>
        <AppFooter>
          <Suspense fallback={this.loading()}>
            <DefaultFooter />
          </Suspense>
        </AppFooter>
        <Modal isOpen={this.props.modalData.isOpenModal} >
          <ModalHeader>{this.props.modalData.title}</ModalHeader>
          <ModalBody>
            {this.props.modalData.content}
          </ModalBody>
          <ModalFooter>
            <Button color="secondary" onClick={this.closeModal}>Close</Button>
          </ModalFooter>
        </Modal>
      </div>
    );
  }
}


function mapStateToProps(state) {
  return {
    authenticationData : state.authenticationData,
    modalData: state.modalData
  };
}

function mapDispatchToProps(dispatch){
  return bindActionCreators({
    toggleModal
  },dispatch);
}


export default connect(mapStateToProps, mapDispatchToProps)(DefaultLayout);


