import React, { Component } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { Badge, UncontrolledDropdown, DropdownItem, DropdownMenu, DropdownToggle, Nav, NavItem } from 'reactstrap';
import PropTypes from 'prop-types';

import { AppAsideToggler, AppNavbarBrand, AppSidebarToggler } from '@coreui/react';
import logo from '../../assets/img/logo/brain.jpg'
import sygnet from '../../assets/img/logo/brain.jpg'
import avatar from '../../assets/img/avatars/icon.png'

import { connect } from 'react-redux';
import { authProvider } from '../../services';

const propTypes = {
  children: PropTypes.node,
};

const defaultProps = {};

class DefaultHeader extends Component {
  render() {
    
    // eslint-disable-next-line
    const { children, ...attributes } = this.props;
    
    const _copyPros = {...this.props};
    delete _copyPros.authenticationData;
    delete _copyPros.dispatch;

    const loginComponent = this.props.authenticationData.aadResponse ? 
      <Nav className="ml-auto" navbar>
        <UncontrolledDropdown nav direction="down">
          <DropdownToggle nav>
            Xin chào, {this.props.authenticationData.aadResponse.account.name}{' '}<img src={avatar} className="img-avatar" alt={'icon'} />
          </DropdownToggle>
          <DropdownMenu right>
            <DropdownItem>{this.props.authenticationData.aadResponse.account.name}</DropdownItem>
            <DropdownItem onClick={authProvider.logout}><i className="fa fa-lock"></i> Logout</DropdownItem>
          </DropdownMenu>
        </UncontrolledDropdown>
      </Nav>
      :
      <>
        <Nav className="ml-auto" navbar>
        </Nav>
        <a className="nav-link" onClick={authProvider.login} style={{cursor : "hand"}}>Login</a>
      </>

    return (
      <React.Fragment>
        <AppSidebarToggler className="d-lg-none" display="md" mobile />
        <AppNavbarBrand
          full={{ src: logo, width: 89, height: 25, alt: 'Novaland Logo' }}
          minimized={{ src: sygnet, width: 30, height: 30, alt: 'Novaland Logo' }}
        />
        <AppSidebarToggler className="d-md-down-none" display="lg" />

        {loginComponent}
        
        
      </React.Fragment>
    );
  }
}

DefaultHeader.propTypes = propTypes;
DefaultHeader.defaultProps = defaultProps;


function mapStateToProps(state) {
  return {
    authenticationData : state.authenticationData
  };
}

export default connect(mapStateToProps)(DefaultHeader);

