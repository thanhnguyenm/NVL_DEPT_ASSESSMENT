import React, { Component, lazy, Suspense } from 'react';
import { Pagination, PaginationItem, PaginationLink } from 'reactstrap';

class CustomPagination extends Component {
    constructor(props) {
        super(props);    
    }

    render() {

        let activePage = this.props.activePage;
        let itemsPerPage = this.props.itemsPerPage;
        let totalItemsCount = this.props.totalItemsCount;
        let onChange = this.props.onChange;

        let allPages = Math.floor(totalItemsCount / itemsPerPage);
        if(totalItemsCount > allPages * itemsPerPage) allPages++;

        let fromPage = activePage - 10;
        let toPage = activePage + 10;
        if(fromPage < 1) fromPage = 1;
        if(toPage > allPages) toPage = allPages;


        let items = [];
        for (let number = 1; number <= allPages; number++) {
            if(number === activePage){
                items.push(
                    <PaginationItem key={number} active>
                        <PaginationLink tag="button">{number}</PaginationLink>
                    </PaginationItem>
                );
            } else {
                items.push(
                    <PaginationItem key={number} onClick={e => onChange(number, e)}>
                        <PaginationLink tag="button">{number}</PaginationLink>
                    </PaginationItem>
                );
            }
            
        }

        const divStyle={
            overflow: 'auto'
          };

        return(
            

            <div style={divStyle}>
                <Pagination>
                    {items}
                </Pagination>
            </div>
        )
    }
}

export { CustomPagination };