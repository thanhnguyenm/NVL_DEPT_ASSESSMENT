import React, { Component } from 'react';


import logo from '../../assets/img/logo/brain.jpg';

class Dashboard extends Component {
  constructor(props) {
    super(props);

  }


  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>

  render() {

    return (
      <div className="animated fadeIn">
        <div style={{textAlign: "center"}}>
          <h3>ĐÁNH GIÁ CHẤT LƯỢNG DỊCH VỤ NỘI BỘ CÁC PHÒNG BAN</h3> <br/>
          <img src={logo} alt="" />
        </div>
        
      </div>
    );
  }
}

export default Dashboard;
