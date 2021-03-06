import React from "react";
import * as productService from "../../../services/products/productService";
import * as styles from "./urlPreview.module.css";
import PropTypes from "prop-types";
import logger from "../../../logger";
import MapProducts from "./MapProducts";

const _logger = logger.extend("ProductPreview");

class ProductPreview extends React.Component {
  state = {
    product: null,
    productArr: []
  };

  componentDidMount() {
    _logger(this.props.url);
    if (this.props.url) {
      for (let i = 0; i < this.props.url.length; i++) {
        const newString = this.props.url[i];
        const newArray = newString.match(/\d+/g);
        const productId = newArray[1];

        productService
          .getByIdProduct(productId)
          .then(this.onGetByIdSuccess)
          .then(this.mapProductsPreview)
          .catch(this.onGetByError);
      }
    }
  }

  onGetByIdSuccess = response => {
    const product = response.item;
    this.setState(prevState => {
      if (prevState.productArr[0]) {
        const totalProducts = prevState.productArr.concat(product);
        return { product, productArr: totalProducts };
      } else {
        const newArr = new Array(product);
        return {
          product,
          productArr: newArr
        };
      }
    });
  };

  onGetByIdError = error => {
    _logger(error);
  };

  checkForHttp = url => {
    const checkHttps = url.slice(0, 4);
    if (checkHttps !== "http") {
      return `https://${url}`;
    } else {
      return url;
    }
  };

  render() {
    return (
      <React.Fragment>
        {this.state.product && (
          <div>
            <div
              className={`${styles.messagePassed}`}
              dangerouslySetInnerHTML={{ __html: this.props.message }}
            />
            <MapProducts
               products={this.state.productArr}
               url={this.props.url}
               checkForHttp={this.checkForHttp}
            />
          </div>
        )}
      </React.Fragment>
    );
  }
}
ProductPreview.propTypes = {
  message: PropTypes.string,
  url: PropTypes.array
};
export default ProductPreview;
