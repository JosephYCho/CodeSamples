import React from "react";
import {
  Card,
  CardImg,
  CardText,
  CardBody,
  CardTitle,
  CardSubtitle,
  Button
} from "reactstrap";

class PersonalPetPureComponent extends React.PureComponent {

  handleEdit =()=>{
    this.props.onEditClick(this.props.pet)
  }


  handleDelete =()=>{
    this.props.onDeleteClick(this.props.pets.id)
  }

  render() {
    return (
      <div>
        <Card>
          <CardImg
            className="imageCss"
            top
            width="100%"
            src={this.props.img}
            alt="Pets"
          />
          <CardBody>
            <CardTitle>{this.props.title}</CardTitle>
            <CardSubtitle>{this.props.location}</CardSubtitle>
            <CardText>{this.props.description}</CardText>
            <CardText>Contact Me: {this.props.email}</CardText>
            <Button onClick={this.handleEdit}>
              Edit
            </Button>
            <Button onClick={this.handleDelete}>
              Delete
            </Button>
          </CardBody>
        </Card>
      </div>
    );
  }
}

export default PersonalPetPureComponent;
