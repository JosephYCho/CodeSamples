import React from "react";
import "../../assets/scss/style.css";
import "./ChatTheme.css";
import ProductPreview from "./urlPreview/ProductPreview";
import InfluencerPreview from "./urlPreview/InfluencerPreview";
import ChatLinkPreview from "./urlPreview/ChatLinkPreview";
import PropTypes from "prop-types";
import MapCurrentConversation from "./MapCurrentConversation";
import * as checkUrls from "./checkUrls";

class CurrentConversation extends React.Component {
  componentDidMount() {
    this.scrollToBottom();
  }

  componentDidUpdate() {
    this.scrollToBottom();
  }

  scrollToBottom() {
    this.el.scrollIntoView({
      block: "end"
    });
  } 

  getPreview = message => {
    let content = null;
    const productUrlArr = checkUrls.isProdUrl(message);
    const influencerUrlArr = checkUrls.isInfluencerUrl(message);
    const newMessage = checkUrls.addAnchor(message);

    if (productUrlArr || influencerUrlArr) {
      content = (
        <React.Fragment>
          <ProductPreview message={newMessage} url={productUrlArr} />
          {productUrlArr ? (
            <InfluencerPreview url={influencerUrlArr} />
          ) : (
            <InfluencerPreview message={newMessage} url={influencerUrlArr} />
          )}
        </React.Fragment>
      );
    } else if (!productUrlArr && !influencerUrlArr) {
      content = (
        <InfluencerPreview message={newMessage} url={influencerUrlArr} />
      );
    } else {
      content = message;
    }

    return content;
  };

  

  showWebUrlPreview = message => {
    const webUrl = checkUrls.isWebUrl(message);
    const webMessage = checkUrls.addAnchor(message, webUrl, 0, -1);
    return <ChatLinkPreview message={webMessage} url={webUrl} />;
  };

  //#endregion

  render() {
    const { conversation, currentUserId } = this.props;
    return (
      <ul
        className="chat-list p-20"
        style={{ overflowY: "auto", width: "auto", height: "401px" }}
      >
        <MapCurrentConversation
          conversation={conversation}
          currentUserId={currentUserId}
          getPreview={this.getPreview}
        />
        <div
          ref={el => {
            this.el = el;
          }}
        />
      </ul>
    );
  }
}

CurrentConversation.propTypes = {
  currentUserMessages: PropTypes.array,
  conversation: PropTypes.array,
  currentUserId: PropTypes.number
};

export default CurrentConversation;
