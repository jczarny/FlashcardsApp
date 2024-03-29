import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { AuthContext } from '../contexts/AuthContext';

export class NavMenu extends Component {

    static displayName = NavMenu.name;
    static contextType = AuthContext;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        //console.log(this.context);
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
                    <NavbarBrand tag={Link} to="/">FlashcardsApp</NavbarBrand>
                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            {
                                this.context.isLoggedIn &&
                                <>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/">Learn</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/browse">Browse Decks</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/create">Create New</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/logout">Logout</NavLink>
                                    </NavItem>
                                </>
                            }
                            {
                                !this.context.isLoggedIn &&
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/">Login</NavLink>
                                </NavItem>
                            }
                        </ul>
                    </Collapse>
                </Navbar>
            </header>
        );
    }
}
